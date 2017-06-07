
import { Injectable, EventEmitter } from '@angular/core';
import { BaseService } from './base-service';
import { StockExchange } from '../models/stock-exchange';
import { Agent } from '../models/agent';


@Injectable()
export class StockExchangeService extends BaseService {

    // public events
    public onCreatedAgent : EventEmitter<Agent>;

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('stockExchangeHub');
        this.onCreatedAgent = new EventEmitter<Agent>();

        this.proxy.on('createdAgent', a =>  {
            this.onCreatedAgent.emit(a);
        })

        this.init();
    }

    /**
     * Save an stock exchange
     * @param stock 
     */
    public save(stock: StockExchange) {
        return this.execute('save', stock);
    }

}