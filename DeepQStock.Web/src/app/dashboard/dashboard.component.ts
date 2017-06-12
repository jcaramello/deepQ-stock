import { Component, OnInit, OnDestroy, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { StockExchange } from '../models/stock-exchange';
import { OnDayCompletedArgs } from '../models/on-day-completed-args';
import { AgentService } from '../services/agent-service';
import { StockExchangeService } from '../services/stock-exchange-service';
import { NotificationsService } from 'angular2-notifications';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { ActionType } from '../models/enums';

@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {

  // Private fields
  private sub: any;

  // Public fields
  public agent: Agent = new Agent();
  public stock: StockExchange = new StockExchange();
  public today = new OnDayCompletedArgs();
  public daysCompleted: OnDayCompletedArgs[] = [];
  public days: any[] = [];

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
        .then(a => this.agent = a)
        .then(a => this.stockExchangeService.getById(this.agent.stockExchangeParametersId))
        .then(s => this.stock = s);
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
  public onDayCompleted(args: OnDayCompletedArgs) {    
    if(args){
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
    this.notificationService.info("Info", "Simulacion iniciada");
    this.agentService.start(this.agent.id);
  }
}
