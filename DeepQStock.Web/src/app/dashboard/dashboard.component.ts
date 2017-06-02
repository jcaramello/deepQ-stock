import { Component, OnInit, OnDestroy, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { AgentService } from '../services/agent.service';


@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {


  private sub: any;
  private agent: Agent = new Agent();  

  /**
   * Creates an instance of DashboardComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof DashboardComponent
   */
  constructor(private route: ActivatedRoute, private agentService: AgentService) { }

  /**
   * initialize the component
   * 
   * @memberof DashboardComponent
   */
  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.agentService.getById(+params['id']).then(a => {
        this.agent = a
      })
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
