import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard.component';
import { CandlestickComponent } from '../candlestick/candlestick-component';
import { DashboardRoutingModule } from './dashboard-routing.module';

@NgModule({
  imports: [
    DashboardRoutingModule, CommonModule   
  ],
  declarations: [ DashboardComponent, CandlestickComponent ]
})
export class DashboardModule { }
