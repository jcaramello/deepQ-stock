import { Component, HostListener, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { StockExchangeService } from '../services/stock-exchange-service';
import { Agent } from '../models/agent';
import { SlimLoadingBarService } from 'ng2-slim-loading-bar';

/**
* Allows to draw a candlestick chart
*/
@Component({
  selector: 'candlestick',
  template: '<div id="candlestick-container"></div>'
})
export class CandlestickComponent {

  @Input()
  public agent;

   @Input()
  public stock;

  private chart: AmCharts.AmStockChart;
  private pricePanel: AmCharts.StockPanel;
  private volPanel: AmCharts.StockPanel;
  private data: any[];

  /**
   * Creates an instance of BreadcrumbsComponent.
   * @param {Router} router 
   * @param {ActivatedRoute} route 
   * 
   * @memberof BreadcrumbsComponent
   */
  constructor(private router: Router, private route: ActivatedRoute, private stockExchangeService: StockExchangeService, private slimLoadingBarService: SlimLoadingBarService) {
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
    this.agent = changes['agent'].currentValue;
    this.init();
  }



  /**
   * Initialize the charts
   * 
   * @private
   * 
   * @memberof CandleStickComponent
   */
  private init() {

    if (!this.agent || !this.agent.id) return;

    var chart = this.chart = new AmCharts.AmStockChart();
    chart['theme'] = 'light';
    chart['language'] = 'es';
    chart['pathToImages'] = "/bower_components/amcharts3/amcharts/images/";
    chart['dataDateFormat'] = "YYYY/MM/DD";
    chart['mouseWheelScrollEnabled'] = true;
    chart.valueAxesSettings.position = 'right';    

    chart.categoryAxesSettings.equalSpacing = true;
    chart.categoryAxesSettings.groupToPeriods = ["DD"];

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
        this.slimLoadingBarService.complete()   
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
    chart.dataSets = [dataSet];

    var pricePanel = this.pricePanel = new AmCharts.StockPanel();
    pricePanel.mouseWheelZoomEnabled = false;
    pricePanel.mouseWheelScrollEnabled = false;
    pricePanel.categoryField = 'date';
    pricePanel.percentHeight = 80;
    pricePanel.categoryAxis.parseDates = true;

    var legend = new AmCharts.StockLegend();
    pricePanel.stockLegend = legend;

    var panelsSettings = new AmCharts.PanelsSettings();
    chart.panelsSettings = panelsSettings;

    var priceGraph = new AmCharts.StockGraph();
    priceGraph.valueField = "close";
    priceGraph.type = "candlestick";
    priceGraph.lowField = 'low';
    priceGraph.highField = 'high';
    priceGraph.openField = 'open';
    priceGraph.closeField = 'close';
    priceGraph.balloonText = "Open:<b>[[open]]</b><br>Low:<b>[[low]]</b><br>High:<b>[[high]]</b><br>Close:<b>[[close]]</b><br>";
    priceGraph.title = this.agent.symbol;
    priceGraph.fillColors = "#66cc66";
    priceGraph.useDataSetColors = false;
    priceGraph.lineColor = '#595959';
    priceGraph.fillAlphas = 0.9;
    priceGraph.negativeFillColors = "#db4c3c";
    priceGraph.negativeLineColor = "#595959";
    pricePanel.addStockGraph(priceGraph);

    var volGraph = new AmCharts.StockGraph();
    volGraph.valueField = "volume";
    volGraph.type = "column";
    volGraph.showBalloon = false;
    volGraph.comparable = true;
    volGraph.useDataSetColors = false;
    volGraph.fillAlphas = 1
    volGraph.compareField = 'volume';
    volGraph.fillColors = "#ffec63";
    volGraph.lineColor = '#ffe216';

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
    chart.chartScrollbarSettings = sbsettings;

    chart.chartCursorSettings.bulletsEnabled = true;
    chart.chartCursorSettings.valueBalloonsEnabled = true;
    chart.chartCursorSettings.cursorColor = "#8c8c8c"
    chart.chartCursorSettings.zoomable = true;

    var periodSelector = new AmCharts.PeriodSelector();
    periodSelector.periods = [
      { period: "MM", count: 1, label: "1M" },
      { period: "MM", count: 6, label: "6M" },
      { period: "YYYY", count: 1, label: "1A", selected: true },
      { period: "MAX", label: "Max" }
    ];

    chart.periodSelector = periodSelector;

    chart.panels = [pricePanel, volPanel];
    chart.addListener("rendered", this.zoomChart.bind(this));
    chart['write']('candlestick-container');
    this.zoomChart();
  }

  // this method is called when chart is first inited as we listen for "rendered" event
  private zoomChart() {
    // different zoom methods can be used - zoomToIndexes, zoomToDates, zoomToCategoryValues
    var zoomToIndexes: any = this.pricePanel.zoomToIndexes;
    zoomToIndexes(this.data.length - 3, this.data.length - 1);
  }
}
