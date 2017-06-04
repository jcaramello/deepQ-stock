import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';

import { AppComponent } from './app.component';
import { DropdownModule } from 'ng2-bootstrap/dropdown';
import { TabsModule } from 'ng2-bootstrap/tabs';
import { NAV_DROPDOWN_DIRECTIVES } from './shared/nav-dropdown.directive';

import { SIDEBAR_TOGGLE_DIRECTIVES } from './shared/sidebar.directive';
import { AsideToggleDirective } from './shared/aside.directive';
import { BreadcrumbsComponent } from './shared/breadcrumb.component';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { StockExchangeService } from './services/stock.exchange.service';
import { AgentService } from './services/agent.service';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

// Routing Module
import { AppRoutingModule } from './app.routing';

//Layouts
import { FullLayoutComponent } from './layouts/full-layout.component';

@NgModule({
  imports: [
    BrowserModule,  
    BrowserAnimationsModule,
    AppRoutingModule,
    DropdownModule.forRoot(),
    NgbModule.forRoot(),
    TabsModule.forRoot(),
    SimpleNotificationsModule.forRoot()
  ],
  declarations: [
    AppComponent,
    FullLayoutComponent,      
    NAV_DROPDOWN_DIRECTIVES,
    BreadcrumbsComponent,
    SIDEBAR_TOGGLE_DIRECTIVES,
    AsideToggleDirective
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: StockExchangeService, useClass: StockExchangeService },
    { provide: AgentService, useClass: AgentService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
