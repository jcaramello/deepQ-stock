import { Component, HostListener, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import * as moment from 'moment';

/**
* Allows to draw a candlestick this.chart
*/
@Component({
  selector: 'barchart',
  template: '<div id="barchart-container-{{id}}" class="barchart-container"></div>'
})
export class BarChartComponent {

  private static IdGenerator: number = 0;
  private id: number;

  @Input()
  public values: any[];

  @Input()
  public title: string;

  @Input()
  public field: string

  @Input()
  public category: string;

  private chart: any;

  /**
   * Creates an instance of BreadcrumbsComponent.
   * @param {Router} router 
   * @param {ActivatedRoute} route 
   * 
   * @memberof BreadcrumbsComponent
   */
  constructor(private router: Router, private route: ActivatedRoute, private slimLoadingBarService: SlimLoadingBarService) {
    this.values = [];
    this.id = BarChartComponent.IdGenerator++;
  }

  /**
   * 
   * On Initi Directive
   * @memberof BreadcrumbsComponent
   */
  ngOnInit(): void {    
  }

  /**
   * Trigger when inputs changes
   */
  ngOnChanges(changes: SimpleChanges): void {
    this.values = changes['values'] && changes['values'].currentValue;
    if(this.values && this.values.length > 0){
      this.init();
    }  
  }

  /**
   * Initialize the this.charts
   * 
   * @private
   * 
   * @memberof CandleStickComponent
   */
  private init() {

    this.chart = <any>AmCharts.makeChart("barchart-container-" + this.id, {
      "type": "serial",
      "theme": "light",
      "marginRight": 70,
      "dataProvider": this.values,
      "valueAxes": [{
        "axisAlpha": 0,
        "position": "left",
        "title": this.title
      }],
      "startDuration": 1,
      "graphs": [{
        "balloonText": "<b>[[category]]: [[value]]</b>",
        "fillColorsField": "color",
        "fillAlphas": "0.9",
        "lineAlpha": "0.2",
        "type": "column",
        "valueField": this.field
      }],
      "chartCursor": {
        "categoryBalloonEnabled": false,
        "cursorAlpha": 0,
        "zoomable": false
      },
      "categoryField": this.category,
      "categoryAxis": {
        "gridPosition": "start",
        "labelRotation": 45
      },
      "export": {
        "enabled": true
      }

    });   
      
  }
}
