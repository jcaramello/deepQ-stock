
import { Injectable, EventEmitter } from '@angular/core';
import { BaseService } from './base-service';
import { StockExchange } from '../models/stock-exchange';


@Injectable()
export class StockExchangeService extends BaseService {

    // public events

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('stockExchangeHub');        

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