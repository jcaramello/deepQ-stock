import { Component, HostListener } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';

/**
* Allows to draw a candlestick chart
*/
@Component({
  selector: 'candlestick',
  template: '<div class="candlestick-container"></div>'
})
export class CandleStickComponent {

  public static data = [
    { date: new Date(2011, 5, 1, 0, 0, 0, 0), val: 10 },
    { date: new Date(2011, 5, 2, 0, 0, 0, 0), val: 11 },
    { date: new Date(2011, 5, 3, 0, 0, 0, 0), val: 12 },
    { date: new Date(2011, 5, 4, 0, 0, 0, 0), val: 11 },
    { date: new Date(2011, 5, 5, 0, 0, 0, 0), val: 10 },
    { date: new Date(2011, 5, 6, 0, 0, 0, 0), val: 11 },
    { date: new Date(2011, 5, 7, 0, 0, 0, 0), val: 13 },
    { date: new Date(2011, 5, 8, 0, 0, 0, 0), val: 14 },
    { date: new Date(2011, 5, 9, 0, 0, 0, 0), val: 17 },
    { date: new Date(2011, 5, 10, 0, 0, 0, 0), val: 13 }
  ];

  /**
   * Creates an instance of BreadcrumbsComponent.
   * @param {Router} router 
   * @param {ActivatedRoute} route 
   * 
   * @memberof BreadcrumbsComponent
   */
  constructor(private router: Router, private route: ActivatedRoute) { }

  /**
   * 
   * On Initi Directive
   * @memberof BreadcrumbsComponent
   */
  ngOnInit(): void {

    var chart = new AmCharts.AmStockChart();

  }
}
