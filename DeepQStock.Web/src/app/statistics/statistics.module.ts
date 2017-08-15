import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatisticsComponent } from './statistics.component';
import { BarChartComponent } from '../charts/barchart-component';
import { LineChartComponent } from '../charts/linechart-component';
import { StatisticsRoutingModule } from './statistics-routing.module';

@NgModule({
  imports: [
    StatisticsRoutingModule, CommonModule   
  ],  
  declarations: [ StatisticsComponent, BarChartComponent, LineChartComponent ]
})
export class StatisticsModule { }
