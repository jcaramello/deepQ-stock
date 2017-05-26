import { NgModule } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { CandleStickComponent } from '../components/candleStick.component';
import { DashboardRoutingModule } from './dashboard-routing.module';

@NgModule({
  imports: [
    DashboardRoutingModule,
    CandleStickComponent
  ],
  declarations: [ DashboardComponent ]
})
export class DashboardModule { }
