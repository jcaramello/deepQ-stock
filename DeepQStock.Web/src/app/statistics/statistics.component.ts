import { Component, OnInit, OnDestroy, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { OnSimulationComplete } from '../models/on-simulation-complete';
import { AgentService } from '../services/agent-service';
import { NotificationsService } from 'angular2-notifications';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { ActionType, AgentStatus } from '../models/enums';
import * as _ from 'lodash';

@Component({
  templateUrl: 'statistics.component.html'
})
export class StatisticsComponent implements OnInit, OnDestroy {

  // Private fields
  private sub: any;

  // Public fields
  public agents: Agent[] = [];
  public results: OnSimulationComplete[] = [];
  public averageAnnualRent: any[];
  public progressAnnualRentChart: any[];

  /**
   * Creates an instance of StatisticsComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof StatisticsComponent
   */
  constructor(
    private route: ActivatedRoute,
    private agentService: AgentService,
    private notificationService: NotificationsService,
    private slimLoadingBarService: SlimLoadingBarService,
    private zone: NgZone) { }

  /**
   * initialize the component
   * 
   * @memberof StatisticsComponent
   */
  ngOnInit() {
    this.slimLoadingBarService.start();    
    this.agentService.onSimulationCompleted.subscribe(this.onSimulationCompleted.bind(this));    
    this.sub = this.route.params.subscribe(params => this.loadData(+params['id']));


    this.averageAnnualRent = [
      {agentId: 1, annualRent: 13},
      {agentId: 2, annualRent: 4},
      {agentId: 3, annualRent: 8},
      {agentId: 4, annualRent: 43}
    ]    

    this.progressAnnualRentChart = [
      {"annualRent-1": 2, order: 1},
      {"annualRent-1": 7, order: 2},
      {"annualRent-1": 11, order: 3},
      {"annualRent-1": 18, order: 4},
      {"annualRent-1": 16, order: 5},
      {"annualRent-2": 0.78, order: 1},
      {"annualRent-2": 3.4, order: 2},
      {"annualRent-2": 5, order: 3},
    ]
  }

  /**
   * Execute when the component is destroyed
   * @memberof StatisticsComponent
   */
  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  /**
   * Load page data
   * @param agentId 
   */
  public loadData(agentId: number) {
    return this.agentService.getAll().then(a => {
      this.agents = a;
      this.slimLoadingBarService.complete();
    })
  }
  /**
  * Execute when a simulation day is completed
  * @param args 
  */
  public onSimulationCompleted(args: OnSimulationComplete) {
    this.results.push(args);
  }

}
