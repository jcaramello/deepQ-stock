import { Period } from './period';
import { ActionType } from './enums';

/**
 * On day completed arguments
 * 
 * @export
 * @class OnDayComplete
 */
export class OnTrainingEpochComplete {

    public agentId:number;
    public epoch:number;
    public error:number;
}