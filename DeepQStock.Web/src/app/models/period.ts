export enum PeriodType{
    day,
    week, 
    month
}

export class Period{

    public id: number;
    public periodType: PeriodType;
    public date:Date;
    public open:number;
    public close:number;
    public high:number;
    public low:number;
    public volumen: number;
    public currentCapital:number;
    public actualPosition:number;
    public indicators:  { [id: string] : number[]; } 
}