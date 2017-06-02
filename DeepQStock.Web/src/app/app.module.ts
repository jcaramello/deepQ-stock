import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';

import { AppComponent } from './app.component';
import { DropdownModule } from 'ng2-bootstrap/dropdown';
import { TabsModule } from 'ng2-bootstrap/tabs';
import { NAV_DROPDOWN_DIRECTIVES } from './shared/nav-dropdown.directive';

import { SIDEBAR_TOGGLE_DIRECTIVES } from './shared/sidebar.directive';
import { AsideToggleDirective } from './shared/aside.directive';
import { BreadcrumbsComponent } from './shared/breadcrumb.component';

import { StockExchangeService } from './services/stock.exchange.service';
import { AgentService } from './services/agent.service';

// Routing Module
import { AppRoutingModule } from './app.routing';

//Layouts
import { FullLayoutComponent } from './layouts/full-layout.component';

@NgModule({
  imports: [
    BrowserModule,
    AppRoutingModule,
    DropdownModule.forRoot(),
    TabsModule.forRoot(),
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
