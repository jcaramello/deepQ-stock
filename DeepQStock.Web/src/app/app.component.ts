import { Component } from '@angular/core';

@Component({
    selector: 'body',
    template: `<ng2-slim-loading-bar [color]="'#20a8d8'" [height]="'3px'"></ng2-slim-loading-bar>
               <router-outlet></router-outlet>
               <simple-notifications [options]="notificationsOptions"></simple-notifications>`
})
export class AppComponent {
    public notificationsOptions = {
        timeOut: 5000,
        preventDuplicates:true
    }
 }
