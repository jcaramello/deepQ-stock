import { Component, OnInit, OnDestroy, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { StockExchange } from '../models/stock-exchange';
import { OnDayComplete } from '../models/on-day-complete';
import { AgentService } from '../services/agent-service';
import { StockExchangeService } from '../services/stock-exchange-service';
import { NotificationsService } from 'angular2-notifications';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { ActionType } from '../models/enums';
import * as _ from 'lodash';

@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {

  // Private fields
  private sub: any;

  // Public fields
  public agent: Agent = new Agent();
  public stock: StockExchange = new StockExchange();
  public today = new OnDayComplete();
  public daysCompleted: OnDayComplete[] = [];
  public days: any[] = [];
  public isRuning: boolean;

  /**
   * Creates an instance of DashboardComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof DashboardComponent
   */
  constructor(private route: ActivatedRoute,
    private agentService: AgentService,
    private stockExchangeService: StockExchangeService,
    private notificationService: NotificationsService,
    private slimLoadingBarService: SlimLoadingBarService,
    private zone: NgZone) { }

  /**
   * initialize the component
   * 
   * @memberof DashboardComponent
   */
  ngOnInit() {
    this.agentService.onDayCompleted.subscribe(this.onDayCompleted.bind(this));
    this.sub = this.route.params.subscribe(params => {
      this.slimLoadingBarService.start();
      this.agentService
        .getById(+params['id'])
        .then(a => {
          this.agent = a;
          this.today = _.last(a.decisions) || this.today;  
          return a;
        })
        .then(a => this.stockExchangeService.getById(this.agent.stockExchangeParametersId))
        .then(s => this.stock = s)
    });
  }

  /**
   * Execute when the component is destroyed
   * @memberof DashboardComponent
   */
  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  /**
   * Execute when a simulation day is completed
   * @param args 
   */
  public onDayCompleted(args: OnDayComplete) {
    if (this.isRuning && args) {
      args.date = new Date(args.date);
      this.zone.run(() => this.today = args);
    }
  }

  /**
   * Start the agent simulation 
   * @param {any} event 
   * 
   * @memberof DashboardComponent
   */
  public play(event) {
    this.isRuning = !this.isRuning;
    this.notificationService.info("Info", "Simulacion iniciada");
    this.agentService.start(this.agent.id);
  }

  /**
   * Stop the agent simulation
   * @param event 
   */
  public pause(event, stockChart) {
    this.isRuning = !this.isRuning;
    this.notificationService.info("Info", "Simulacion pausada");
    this.agentService.pause(this.agent.id);
  }

  /**
  * Stop the agent simulation
  * @param event 
  */
  public stop(event, stockChart) {
    this.isRuning = !this.isRuning;
    this.notificationService.info("Info", "Simulacion detenida");
    this.agentService.stop(this.agent.id);
    stockChart.clearMarkers();
    this.today = new OnDayComplete();
  }

  /**
   * Reset the agent 
   * @param event 
   */
  public reset(event, stockChart) {
    this.stop(event, stockChart)
    this.notificationService.info("Info", "El agente fue reseteado.");
    this.agentService.reset(this.agent.id);
  }
}
