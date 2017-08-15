import { Period } from './period';
import { ActionType } from './enums';

/**
 * On day completed arguments
 * 
 * @export
 * @class OnDayComplete
 */
export class OnSimulationComplete {

    public agentId:number;
    public symbol: string;
    public createdOn: Date;
    public annualProfits: number;
    public annualRent: number;
    public profits: number;
    public earnings: number;
    public netCapital: number;
    public transactionCost: number;

    
}