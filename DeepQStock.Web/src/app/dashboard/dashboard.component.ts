import { Component, OnInit, OnDestroy, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Agent } from '../models/agent';
import { OnDayCompletedArgs } from '../models/on-day-completed-args';
import { AgentService } from '../services/agent.service';
import { NotificationsService } from 'angular2-notifications';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {

  // Private fields
  private sub: any;

  // Public fields
  public agent: Agent = new Agent();
  public today = new OnDayCompletedArgs();
  public markers: { decision: string, date: Date }[] = [];

  /**
   * Creates an instance of DashboardComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof DashboardComponent
   */
  constructor(private route: ActivatedRoute,
    private agentService: AgentService,
    private notificationService: NotificationsService,
    private modalService: NgbModal) { }

  /**
   * initialize the component
   * 
   * @memberof DashboardComponent
   */
  ngOnInit() {
    this.agentService.onDayCompleted.subscribe(args => this.today = args);
    this.sub = this.route.params.subscribe(params => {
      this.agentService.getById(+params['id']).then(a => this.agent = a);
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
    this.today = args;
    this.markers.push({
      decision: args.selectedAction,
      date: args.date
    });
  }

  /**
   * Start the agent simulation 
   * @param {any} event 
   * 
   * @memberof DashboardComponent
   */
  public play(event, addEditAgent) {

    this.modalService.open(addEditAgent).result.then((result) => {
       this.notificationService.warn("Info", "Simulacion iniciada"); 
    }, (reason) => {
      this.notificationService.info("Info", "Simulacion iniciada");  
    });

    
  }
}
