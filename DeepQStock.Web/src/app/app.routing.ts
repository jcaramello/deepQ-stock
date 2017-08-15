import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

//Layouts
import { FullLayoutComponent } from './layouts/full-layout.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'introduction',
    pathMatch: 'full',
  },
  {
    path: '',
    component: FullLayoutComponent,
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'dashboard/:id',
        loadChildren: './dashboard/dashboard.module#DashboardModule'
      },
      {
        path: 'introduction',
        loadChildren: './introduction/introduction.module#IntroductionModule'
      },
      {
        path: 'statistics',
        loadChildren: './statistics/statistics.module#StatisticsModule'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
