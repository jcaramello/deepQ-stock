import { Period } from './period';

export class OnDayCompletedArgs {

    /**
     * Creates an instance of OnDayCompletedArgs.
     * 
     * @memberof OnDayCompletedArgs
     */
    constructor(){
        this.period = new Period();
    }

    public dayNumber: number;
    public date: Date;
    public selectedAction: string;
    public reward: number;
    public accumulatedProfits: number;
    public annualProfits: number;
    public annualRent: number;
    public totalOfYears: number;
    public period: Period;
}