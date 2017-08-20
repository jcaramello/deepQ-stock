import { Component, HostListener, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import * as moment from 'moment';

/**
* Allows to draw a candlestick this.chart
*/
@Component({
  selector: 'linechart',
  template: '<div id="linechart-container-{{id}}" class="barchart-container"></div>'
})
export class LineChartComponent {

  @Input()
  public values: any[];

  @Input()
  public title: string;

  @Input()
  public field: string

  @Input()
  public category: string;

  @Input()
  public agents: any[];

  private chart: any;

  private static IdGenerator: number = 0;
  private id: number;

  /**
   * Creates an instance of BreadcrumbsComponent.
   * @param {Router} router 
   * @param {ActivatedRoute} route 
   * 
   * @memberof BreadcrumbsComponent
   */
  constructor(private router: Router, private route: ActivatedRoute) {
    this.id = LineChartComponent.IdGenerator++;
    this.values = [];

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
    if (this.values && this.values.length > 0) {
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

    var graphs = [];
    this.agents.forEach(a => {
      graphs.push({
        "bullet": "square",
        "bulletBorderAlpha": 1,
        "bulletBorderThickness": 1,
        "dashLengthField": "dashLength",
        "legendValueText": "[[value]]",
        "title": "Agente " + a.name + " " + a.id,
        "fillAlphas": 0,
        "valueField": this.field + "" + a.id,
        "valueAxis": "annualRentAxis"
      });
    })

    this.chart = <any>AmCharts.makeChart("linechart-container-" + this.id, {
      "type": "serial",
      "theme": "light",
      "legend": {
        "equalWidths": false,
        "useGraphSettings": true,
        "valueAlign": "left",
        "valueWidth": 120
      },
      "dataProvider": this.values,
      "valueAxes": [{
        "id": "annualRentAxis",
        "axisAlpha": 0,
        "gridAlpha": 0,
        "position": "left",
        "title": this.title
      }],
      "graphs": graphs,
      "chartCursor": {        
        "cursorAlpha": 0.1,
        "cursorColor": "#000000",
        "fullWidth": true,
        "valueBalloonsEnabled": false,
        "zoomable": false
      },      
      "categoryField": this.category
    });
  }



}
