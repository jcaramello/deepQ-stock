import { Component, HostListener, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Agent } from '../models/agent';
import { OnDayComplete } from '../models/on-day-complete';
import { StockExchange } from '../models/stock-exchange';
import { Period } from '../models/period';
import { ActionType } from '../models/enums';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';
import * as moment from 'moment';

/**
* Allows to draw a candlestick this.chart
*/
@Component({
  selector: 'candlestick',
  template: '<div id="candlestick-container"></div>'
})
export class CandlestickComponent {

  @Input()
  public agent: Agent;

  @Input()
  public stock: StockExchange;

  @Input()
  public day: OnDayComplete;

  private stockEvents: AmCharts.StockEvent[] = [];

  private chart: AmCharts.AmStockChart;
  private priceGraph: AmCharts.StockGraph;
  private pricePanel: AmCharts.StockPanel;
  private volPanel: AmCharts.StockPanel;
  private data: any[];
  private initialized: boolean;
  private firstDate: moment.Moment;
  private endDate: moment.Moment;
  private renderComplete;
  private numberOfScroll = 0;
  private initForAgent: number;

  /**
   * Creates an instance of BreadcrumbsComponent.
   * @param {Router} router 
   * @param {ActivatedRoute} route 
   * 
   * @memberof BreadcrumbsComponent
   */
  constructor(private router: Router, private route: ActivatedRoute, private slimLoadingBarService: SlimLoadingBarService) {
    this.data = [];

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
    var currentAgent = changes['agent'] && changes['agent'].currentValue;    
    var day = <OnDayComplete>(changes['day'] && changes['day'].currentValue);
    var newEvent = false;

    if (currentAgent && currentAgent.id && (!this.initialized || currentAgent.id != this.initForAgent)) {
      this.init();
      this.agent = currentAgent;
      this.initialized = true;
      this.initForAgent = this.agent.id;
      
    } else if (this.renderComplete) {

      if (day && day.selectedAction != ActionType.Wait) {
        var event = {
          date: new Date(day.period.date),

          type: "sign",
          graph: this.priceGraph,
          text: day.selectedAction == ActionType.Buy ? "C" : "V",
          backgroundColor: day.selectedAction == ActionType.Buy ? "#20a8d8" : "#FFBF00",
          description: ""
        };

        this.stockEvents.push(<any>event);
        newEvent = true;
      }

      if (day && day.dayNumber > 25) {

        this.firstDate = moment(day.date).add(-90, 'days');
        this.endDate = moment(day.date).add(90, 'days');

        this.chart.zoom(this.firstDate.toDate(), this.endDate.toDate());
      }

      newEvent && this.chart.validateData();
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

    this.chart = new AmCharts.AmStockChart();
    this.chart['theme'] = 'light';
    this.chart['language'] = 'es';
    this.chart['pathToImages'] = "/bower_components/amCharts3/amCharts/images/";
    this.chart['dataDateFormat'] = "YYYY/MM/DD";
    this.chart['mouseWheelScrollEnabled'] = true;
    this.chart.glueToTheEnd = false;
    this.chart.categoryAxesSettings.equalSpacing = true;
    this.chart.valueAxesSettings.position = "right";
    this.chart.categoryAxesSettings.groupToPeriods = ["DD"];

    var loader = {
      url: "assets/data/" + this.stock.symbol + ".csv",
      format: "csv",
      showCurtain: true,
      showErrors: true,
      async: true,
      reverse: true,
      delimiter: ",",
      useColumnNames: true,
      load: (opt, chart) => {
        this.slimLoadingBarService.complete();
      }
    };

    var dataSet = new AmCharts.DataSet();
    dataSet['dataLoader'] = loader;
    dataSet.fieldMappings = [
      { fromField: "date", toField: "date" },
      { fromField: "close", toField: "close" },
      { fromField: "volume", toField: "volume" },
      { fromField: "open", toField: "open" },
      { fromField: "high", toField: "high" },
      { fromField: "low", toField: "low" }
    ];
    dataSet.categoryField = "date";

    var pricePanel = this.pricePanel = new AmCharts.StockPanel();
    pricePanel.mouseWheelZoomEnabled = false;
    pricePanel.mouseWheelScrollEnabled = false;
    pricePanel.categoryField = 'date';
    pricePanel.percentHeight = 80;
    pricePanel.categoryAxis.parseDates = true;

    this.chart.dataSets = [dataSet];
    var legend = new AmCharts.StockLegend();
    pricePanel.stockLegend = legend;

    var panelsSettings = new AmCharts.PanelsSettings();
    this.chart.panelsSettings = panelsSettings;

    var priceGraph = this.priceGraph = new AmCharts.StockGraph();
    priceGraph.valueField = "close";
    priceGraph.type = "candlestick";
    priceGraph.lowField = 'low';
    priceGraph.highField = 'high';
    priceGraph.openField = 'open';
    priceGraph.closeField = 'close';
    priceGraph.balloonText = "Open:<b>[[open]]</b><br>Low:<b>[[low]]</b><br>High:<b>[[high]]</b><br>Close:<b>[[close]]</b><br>";
    priceGraph.title = this.stock.symbol;
    priceGraph.fillColors = "#66cc66";
    priceGraph.useDataSetColors = false;
    priceGraph.lineColor = '#595959';
    priceGraph.fillAlphas = 0.9;
    priceGraph.negativeFillColors = "#db4c3c";
    priceGraph.negativeLineColor = "#595959";
    pricePanel.addStockGraph(priceGraph);

    dataSet.stockEvents = this.stockEvents = this.agent.decisions.map(d => {
      return <any>{
        date: new Date(d.date),
        type: "sign",
        graph: this.priceGraph,
        text: d.selectedAction == ActionType.Buy ? "C" : "V",
        backgroundColor: d.selectedAction == ActionType.Buy ? "#20a8d8" : "#FFBF00",
        description: ""
      };
    });

    var volGraph = new AmCharts.StockGraph();
    volGraph.valueField = "volume";
    volGraph.type = "column";
    volGraph.showBalloon = false;
    volGraph.comparable = true;
    volGraph.useDataSetColors = false;
    volGraph.fillAlphas = 1
    volGraph.compareField = 'volume';
    volGraph.fillColors = "#ffec63";
    volGraph.lineColor = '#ffe216'

    var volPanel = this.volPanel = new AmCharts.StockPanel();
    volPanel.mouseWheelZoomEnabled = true;
    volPanel.mouseWheelScrollEnabled = false;
    volPanel.percentHeight = 20;
    volPanel.addStockGraph(volGraph);

    var volLegend = new AmCharts.StockLegend();
    volLegend.labelText = "Volumen"
    volPanel.stockLegend = volLegend;

    var sbsettings = new AmCharts.ChartScrollbarSettings();
    sbsettings.graph = priceGraph;
    sbsettings.autoGridCount = false;
    sbsettings.updateOnReleaseOnly = false;
    sbsettings['usePeriod'] = 'DD'
    sbsettings.graphType = 'line';
    this.chart.chartScrollbarSettings = sbsettings;

    this.chart.chartCursorSettings.bulletsEnabled = true;
    this.chart.chartCursorSettings.valueBalloonsEnabled = true;
    this.chart.chartCursorSettings.cursorColor = "#8c8c8c"
    this.chart.chartCursorSettings.zoomable = true;
    this.chart.chartCursorSettings['valueZoomable'] = true;

    var periodSelector = new AmCharts.PeriodSelector();
    periodSelector.inputFieldsEnabled = false;
    periodSelector.selectFromStart = true;
    periodSelector.periods = [
      { period: "DD", count: 180, label: "6M", selected: true },
      { period: "YYYY", count: 1, label: "1A" },
      { period: "YYYY", count: 3, label: "3A" },
      { period: "YYYY", count: 5, label: "5A" },
      { period: "MAX", label: "Max" }
    ];

    this.chart.periodSelector = periodSelector;
    this.chart.panels = [pricePanel, volPanel];
    this.chart.addListener('rendered', this.onRendered.bind(this));

    this.chart['write']('candlestick-container');

  }

  // this method is called when this.chart is first inited as we listen for "rendered" event
  private onRendered(event) {

    this.renderComplete = true;
    this.firstDate = moment(this.chart['firstDate']);
    this.endDate = moment(this.firstDate).add(240, 'days');
    this.data = this.chart.dataSets[0].dataProvider;

  }

  /**
   * Clear all markers
   */
  public clearMarkers() {
    this.chart.dataSets[0].stockEvents = this.stockEvents = [];
    this.firstDate = moment(this.chart['firstDate']);
    this.endDate = moment(this.firstDate).add(180, 'days');
    this.chart.zoom(this.firstDate.toDate(), this.endDate.toDate());
    this.chart.validateData();
  }
}
