import { Period } from './period';

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
    public selectedAction: string;
    public reward: number;
    public accumulatedProfits: number;
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