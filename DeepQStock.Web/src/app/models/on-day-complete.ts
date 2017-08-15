import { Period } from './period';
import { ActionType } from './enums';

/**
 * On day completed arguments
 * 
 * @export
 * @class OnDayComplete
 */
export class OnDayComplete {

    // Public Properties
    public dayNumber: number;
    public date: Date;
    public selectedAction: ActionType;
    public reward: number;
    public accumulatedProfit: number;
    public annualProfits: number;
    public annualRent: number;
    public totalOfYears: number;
    public period: Period;
    public volumeOperated:number;

    /**
     * Creates an instance of OnDayComplete.
     * 
     * @memberof OnDayComplete
     */
    constructor() {
        this.period = new Period();        
    }
}