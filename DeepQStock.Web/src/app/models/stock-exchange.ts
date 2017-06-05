import { Agent } from './agent';

/**
 * Stock Exchange
 * 
 * @export
 * @class StockExchange
 */
export class StockExchange {

    // Public Properties
    public id:number;
    public agent: Agent;
    public episodeLength: number;
    public numberOfPeriods: number;
    public initialCapital: number;
    public transactionCost: number;
    public simulationVelocity: number;

    /**
     * Creates an instance of StockExchange.
     * 
     * @memberof StockExchange
     */
    constructor(){
        this.agent = new Agent();
        this.episodeLength = 7;
        this.numberOfPeriods = 14;
        this.initialCapital = 100000;
        this.transactionCost = 0.01;
        this.simulationVelocity = 0.5;        
    }
}