import { Component } from '@angular/core';

@Component({
    selector: 'body',
    template: `<router-outlet></router-outlet>
               <simple-notifications [options]="notificationsOptions"></simple-notifications>`
})
export class AppComponent {
    public notificationsOptions = {
        timeOut: 5000,
        preventDuplicates:true
    }
 }
