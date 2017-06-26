import { Component, OnInit, NgZone } from '@angular/core';
import { AgentService } from '../services/agent-service';
import { StockExchangeService } from '../services/stock-exchange-service';
import { Agent } from '../models/agent';
import { StockExchange } from '../models/stock-exchange';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import { NotificationsService } from 'angular2-notifications';


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
    this.agentService.onCreatedAgent.subscribe(a => this.zone.run(() => this.agents.push(a)));
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
   * Save the new stock
   * @memberof FullLayoutComponent
   */
  save(modal) {
    this.slimLoadingBarService.start();
    this.currentStock.simulationVelocity
    this.stockExchangeService.save(this.currentStock)
      .then(id => {
        this.currentAgent.stockExchangeParametersId = id;
        this.agentService.save(this.currentAgent);
      })
      .then(() => {
        modal.hide();
        this.slimLoadingBarService.complete();
        this.notificationService.success('Creacion Exitosa', 'El agente esta listo para ejecutarse.')
      });
  }


}
