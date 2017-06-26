import { Period } from './period';
import { ActionType } from './enums';

/**
 * On day completed arguments
 * 
 * @export
 * @class OnDayCompletedArgs
 */
export class OnDayCompletedArgs {

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

    /**
     * Creates an instance of OnDayCompletedArgs.
     * 
     * @memberof OnDayCompletedArgs
     */
    constructor() {
        this.period = new Period();        
    }
}