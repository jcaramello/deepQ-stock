import { NgModule } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { CandlestickComponent } from '../components/candlestick.component';
import { DashboardRoutingModule } from './dashboard-routing.module';

@NgModule({
  imports: [
    DashboardRoutingModule    
  ],
  declarations: [ DashboardComponent, CandlestickComponent ]
})
export class DashboardModule { }
