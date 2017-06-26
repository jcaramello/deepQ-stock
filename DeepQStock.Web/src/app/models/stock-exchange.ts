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
    public episodeLength: number;
    public numberOfPeriods: number;
    public initialCapital: number;
    public transactionCost: number;

    get simulationVelocityType(){
        return this.simulationVelocity / 1000;
    }
    set simulationVelocityType(value){
        this.simulationVelocity = value * 1000;
    }

    public simulationVelocity: number;
    public symbol:string;

    /**
     * Creates an instance of StockExchange.
     * 
     * @memberof StockExchange
     */
    constructor(){    
        this.symbol = 'APPL';    
        this.episodeLength = 7;
        this.numberOfPeriods = 14;
        this.initialCapital = 100000;
        this.transactionCost = 0.01;
        this.simulationVelocity = 500;        
    }
}