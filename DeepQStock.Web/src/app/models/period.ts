import { PeriodType } from './enums';
/**
 * Period
 * 
 * @export
 * @class Period
 */
export class Period {

    // Public Properties
    public id: number;
    public periodType: PeriodType;
    public date: Date;
    public open: number;
    public close: number;
    public high: number;
    public low: number;
    public volumen: number;
    public currentCapital: number;
    public actualPosition: number;
    public indicators: { [id: string]: number[]; }
}