import { Component, OnInit, OnDestroy, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { StockExchange } from '../models/stock-exchange';
import { OnDayComplete } from '../models/on-day-complete';
import { OnSimulationComplete } from '../models/on-simulation-complete';
import { OnTrainingEpochComplete } from '../models/on-training-epoch-complete';
import { AgentService } from '../services/agent-service';
import { NotificationsService } from 'angular2-notifications';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { ActionType, AgentStatus } from '../models/enums';
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
  public training = new OnTrainingEpochComplete();
  public daysCompleted: OnDayComplete[] = [];
  public days: any[] = [];

  /**
   * Creates an instance of DashboardComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof DashboardComponent
   */
  constructor(private route: ActivatedRoute,
    private agentService: AgentService,    
    private notificationService: NotificationsService,
    private slimLoadingBarService: SlimLoadingBarService,
    private zone: NgZone) { }

  /**
   * initialize the component
   * 
   * @memberof DashboardComponent
   */
  ngOnInit() {
    this.slimLoadingBarService.start();
    this.agentService.onDayCompleted.subscribe(this.onDayCompleted.bind(this));
    this.agentService.onSimulationCompleted.subscribe(this.onSimulationCompleted.bind(this));
    this.agentService.onTrainingEpochCompleted.subscribe(this.onTrainingEpochCompleted.bind(this));
    this.sub = this.route.params.subscribe(params => this.loadData(+params['id']));
  }

  /**
   * Execute when the component is destroyed
   * @memberof DashboardComponent
   */
  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  /**
   * Load page data
   * @param agentId 
   */
  public loadData(agentId: number) {
    return this.agentService
      .getById(agentId)
      .then(a => {
        this.agent = a;
        this.today = _.last(a.decisions) || this.today;
        if (a.status == AgentStatus.Running) {
          this.agentService.subscribe(agentId);
        }
        this.stock = a.stockExchange;
        return a;
      })            
  }

  /**
   * Execute when a simulation day is completed
   * @param args 
   */
  public onDayCompleted(args: OnDayComplete) {
    if (this.agent.status == AgentStatus.Running && args) {
      args.date = new Date(args.date);
      this.zone.run(() => this.today = args);
    }
  }

  /**
  * Execute when a simulation day is completed
  * @param args 
  */
  public onSimulationCompleted(args: OnSimulationComplete) {
    if (args.agentId == this.agent.id) {
      this.agent.status == AgentStatus.Completed;
    }
  }

  /**
   * 
   * @param args Execute when a training epoch is completed
   */
  public onTrainingEpochCompleted(args: OnTrainingEpochComplete) {
    this.zone.run(() => this.training = args);
  }

  /**
   * Start the agent simulation 
   * @param {any} event 
   * 
   * @memberof DashboardComponent
   */
  public play(event, stockChart) {
    if (this.agent.status == AgentStatus.Completed) {
      stockChart.clearMarkers();
    }
    this.agent.status = AgentStatus.Running
    this.notificationService.info("Info", "Simulacion iniciada");
    this.agentService.start(this.agent.id);
  }

  /**
   * Stop the agent simulation
   * @param event 
   */
  public pause(event, stockChart) {
    this.agent.status = AgentStatus.Paused
    this.notificationService.info("Info", "Simulacion pausada");
    this.agentService.pause(this.agent.id);
  }

  /**
  * Stop the agent simulation
  * @param event 
  */
  public stop(event, stockChart) {
    this.agent.status = AgentStatus.Stoped
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
