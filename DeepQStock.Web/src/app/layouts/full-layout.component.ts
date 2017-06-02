import { Component, OnInit } from '@angular/core';
import { AgentService } from '../services/agent.service';
import { Agent } from '../models/agent';

@Component({
  selector: 'app-dashboard',
  templateUrl: './full-layout.component.html'
})
export class FullLayoutComponent implements OnInit {


  // Public Properties
  public agents: Agent[] = [];
  public disabled: boolean = false;
  public status: { isopen: boolean } = { isopen: false };

  /**
   * Creates an instance of FullLayoutComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof FullLayoutComponent
   */
  constructor(private agentService: AgentService) { }



  /**
   * Toogle left panel
   * 
   * @param {MouseEvent} $event 
   * 
   * @memberof FullLayoutComponent
   */
  public toggleDropdown($event: MouseEvent): void {
    $event.preventDefault();
    $event.stopPropagation();
    this.status.isopen = !this.status.isopen;
  }

  /**
   * on toogle panel
   * 
   * @param {boolean} open 
   * 
   * @memberof FullLayoutComponent
   */
  public toggled(open: boolean): void {

  }


  /**
   * Initialize the component
   * 
   * 
   * @memberof FullLayoutComponent
   */
  ngOnInit(): void {
    this.agentService.getAll().then(a => this.agents = a);
  }
}
