import { Component, OnInit, NgZone } from '@angular/core';
import { AgentService } from '../services/agent-service';
import { StockExchangeService } from '../services/stock-exchange-service';
import { Agent } from '../models/agent';
import { StockExchange } from '../models/stock-exchange';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { NotificationsService } from 'angular2-notifications';
import * as _ from 'lodash';


@Component({
  selector: 'app-dashboard',
  templateUrl: './full-layout.component.html'
})
export class FullLayoutComponent implements OnInit {


  // Public Properties
  public agents: Agent[] = [];
  public disabled: boolean = false;
  public status: { isopen: boolean } = { isopen: false };
  public currentStock: StockExchange = new StockExchange();
  public currentAgent: Agent = new Agent();
  public agentToRemove: Agent;

  /**
   * Companies  
   * @memberof FullLayoutComponent
   */
  public companies = [
    { name: 'Apple', symbol: 'APPL' },
    { name: 'Microsoft', symbol: 'MSFT' },
    { name: 'Walt-Mart', symbol: 'WMT' },
    { name: 'Bayer', symbol: 'BAYRY' },
    { name: 'General Motors', symbol: 'GM' },
    { name: 'J.P. Morgan', symbol: 'JPM' },
    { name: 'Viacom', symbol: 'VIA' },
    { name: 'Exxon Mobile', symbol: 'XOM' }
  ]

  /**
   * Creates an instance of FullLayoutComponent.
   * @param {AgentService} agentService 
   * 
   * @memberof FullLayoutComponent
   */
  constructor(private agentService: AgentService,
    private stockExchangeService: StockExchangeService,
    private slimLoadingBarService: SlimLoadingBarService,
    private notificationService: NotificationsService,
    private zone: NgZone) { }

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
   * @memberof FullLayoutComponent
   */
  ngOnInit(): void {

    this.slimLoadingBarService.start();
    this.agentService.getAll().then(a => this.agents = a).then(() => this.slimLoadingBarService.complete());
    this.agentService.onCreatedAgent.subscribe(this.onCreatedAgent.bind(this));
  }


  /**   
   * Register the new agent
   * @param {Agent} newAgent 
   * @memberof FullLayoutComponent
   */
  onCreatedAgent(agent: Agent) {
    var existingAgent = this.agents.filter(a => a.id == agent.id)[0];
    if (existingAgent) {
      _.remove(this.agents, a => a.id == agent.id);
    }

    this.zone.run(() => this.agents.push(agent));
  }

  /**
   * Add or edit a new agent
   */
  add(modal) {
    this.currentStock = new StockExchange();
    this.currentAgent = new Agent();
    modal.show();
  }

  /**
   * Confirm Elimination
   * @param {any} modal 
   * @param {any} agent 
   * @memberof FullLayoutComponent
   */
  confirmElimination(modal, agent) {
    this.agentToRemove = agent;
    modal.show();
  }

  /**
   * Save the new stock
   * @memberof FullLayoutComponent
   */
  save(modal) {
    this.slimLoadingBarService.start();
    this.stockExchangeService.save(this.currentStock)
      .then(id => {
        this.currentAgent.stockExchangeId = id;
        this.agentService.save(this.currentAgent);
        this.currentAgent.stockExchange = this.currentStock;
      })
      .then(() => {
        modal.hide();
        this.slimLoadingBarService.complete();
        this.notificationService.success('Creacion Exitosa', 'El agente esta listo para ejecutarse.')
      });
  }

  /**
   * Edit an agent
   * @param agentId
   */
  edit(modal, agent: Agent) {
    this.currentAgent = agent;
    this.currentStock = agent.stockExchange;
    modal.show();
  }

  /**
   * Remove an agent 
   * @memberof FullLayoutComponent
   */
  remove(modal) {
    this.slimLoadingBarService.start();
    this.agentService.remove(this.agentToRemove.id).then(() =>{
       this.slimLoadingBarService.complete();
       this.zone.run(() => {
          var idx = this.agents.indexOf(this.agentToRemove);
          this.agents.splice(idx, 1);
          modal.hide();
       });       
    });        
  }

}
