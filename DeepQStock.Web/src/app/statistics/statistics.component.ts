import { Component, OnInit, OnDestroy, EventEmitter, Input, Output, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { SimulationResult } from '../models/simulation-result';
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
  public results: SimulationResult[] = [];
  public averageAnnualRent: any[] = [];
  public progressAnnualRent: any[] = [];

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

    this.agentService.getAll().then(a => this.agents = a).then(() => {

      this.agentService
        .getAllResults()
        .then(r => this.results = r)
        .then(() => this.slimLoadingBarService.complete())
        .then(() => {

          var average = [];
          var progress = []

          this.agents.forEach(a => {

            var agentResults = this.results.filter(r => r.agentId == a.id);
            if (agentResults.length > 0) {
              average.push({
                agentId: a.id,
                name: "#" + a.id + " - " + a.name + " (" + agentResults[0].symbol + ")",
                annualRent: (_.meanBy(agentResults, (p) => p.annualRent) * 100).toFixed(2)
              });
            }
          });

          var maxTotalYear = _.max(_.values(_.groupBy(this.results, r => r.agentId)).map(a => a.length));

          for (var index = 1; index <= maxTotalYear; index++) {
            var record = {
              year: index,
              name: "Simulacion " + index,
            }

            this.agents.forEach(a => {
              var orderedResults = _.orderBy(this.results.filter(r => r.agentId == a.id), (r: SimulationResult) => r.createdOn);
              record["value" + a.id] = orderedResults && orderedResults[index - 1] && (orderedResults[index - 1].annualRent * 100).toFixed(2);
            })

            progress.push(record)

          }


          this.zone.run(() => {
            this.averageAnnualRent = average;
            this.progressAnnualRent = progress;
          })

        });

    })
  }

  /**
   * Execute when the component is destroyed
   * @memberof StatisticsComponent
   */
  ngOnDestroy() {

  }


}
