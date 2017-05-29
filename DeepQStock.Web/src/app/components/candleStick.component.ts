import { Component, HostListener } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { StockExchangeService } from '../services/stock.exchange.service';

/**
* Allows to draw a candlestick chart
*/
@Component({
  selector: 'candlestick',
  template: '<div id="candlestick-{{id}}" class="candlestick-container"></div>'
})
export class CandlestickComponent {

  public static IdGenerator = 1;
  public id: number;

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
  constructor(private router: Router, private route: ActivatedRoute, private stockExchange: StockExchangeService) {
    this.id = CandlestickComponent.IdGenerator++;
    this.data = this.stockExchange.getPeriods();
  }

  /**
   * 
   * On Initi Directive
   * @memberof BreadcrumbsComponent
   */
  ngOnInit(): void {

    AmCharts.ready(this.initializeCharts.bind(this));

  }

  /**
   * Initialize the charts
   * 
   * @private
   * 
   * @memberof CandleStickComponent
   */
  private initializeCharts() {

    var chart = this.chart = new AmCharts.AmStockChart();
    chart.theme = 'light';
    chart.pathToImages = "/bower_components/amcharts3/amcharts/images/";
    chart.valueAxesSettings.position = 'right';
    chart.chartScrollbarSettings.enabled = false;
    chart.categoryAxesSettings.minPeriod = 'DD';
    chart.categoryAxesSettings.equalSpacing = true;

    var dataSet = new AmCharts.DataSet();
    dataSet.dataProvider = this.data;
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
    pricePanel.mouseWheelZoomEnabled = true;
    pricePanel.mouseWheelScrollEnabled = false;
    pricePanel.categoryField = 'date';
    pricePanel.percentHeight = 80;
    pricePanel.categoryAxis.parseDates = true;

    var volPanel = this.volPanel = new AmCharts.StockPanel();
    volPanel.mouseWheelZoomEnabled = true;
    volPanel.mouseWheelScrollEnabled = false;
    volPanel.percentHeight = 20;    

    var legend = new AmCharts.StockLegend();
    pricePanel.stockLegend = legend;

    var panelsSettings = new AmCharts.PanelsSettings();
    panelsSettings.startDuration = 1;

    chart.panelsSettings = panelsSettings;

    var graph = new AmCharts.StockGraph();
    graph.valueField = "close";
    graph.type = "candlestick";
    graph.lowField = 'low';
    graph.highField = 'high';
    graph.openField = 'open';
    graph.closeField = 'close';
    graph.balloonText = "Open:<b>[[open]]</b><br>Low:<b>[[low]]</b><br>High:<b>[[high]]</b><br>Close:<b>[[close]]</b><br>";
    graph.title = "APPL";
    graph.fillColors = "#66cc66";
    graph.useDataSetColors = false;
    graph.lineColor = '#595959';
    graph.fillAlphas = 0.9;
    graph.negativeFillColors = "#db4c3c";
    graph.negativeLineColor = "#595959";

    pricePanel.addStockGraph(graph);

    var volGraph = new AmCharts.StockGraph();
    volGraph.valueField = "close";
    volGraph.type = "column";
    volGraph.showBalloon = false;
    volGraph.comparable = true;
    volGraph.fillColors = '#ffeb99';
    volGraph.useDataSetColors = false;
    volGraph.fillAlphas = 1
    volGraph.compareField = 'close';
    volGraph.fillColors = "#ffeb99";
    volGraph.lineColor = '#ffeb99';

    volPanel.addStockGraph(volGraph);

    var sbsettings = new AmCharts.ChartScrollbarSettings();
    sbsettings.graph = graph;
    sbsettings.autoGridCount = true;
    sbsettings.graphType = 'line';
    chart.chartScrollbarSettings = sbsettings;

    chart.chartCursorSettings.bulletsEnabled = true;
    chart.chartCursorSettings.valueBalloonsEnabled = true;
    chart.chartCursorSettings.zoomable = true;

    var periodSelector = new AmCharts.PeriodSelector();
    chart.periodSelector = periodSelector;

    periodSelector.periods = [
      { period: "DD", count: 1, label: "Dia" },
      { period: "DD", count: 5, label: "Semana" },
      { period: "MM", count: 1, label: "Mes" },
      { period: "YYYY", count: 1, label: "Año" },
      { period: "MAX", selected: true, label: "Max" }
    ];

    chart.periodSelector = periodSelector;
    chart.panels = [pricePanel, volPanel];
    chart.addListener("rendered", this.zoomChart.bind(this));
    this.zoomChart();    

    chart.write('candlestick-' + this.id);
    this.chart = chart;
  }

  // this method is called when chart is first inited as we listen for "rendered" event
  private zoomChart() {
    // different zoom methods can be used - zoomToIndexes, zoomToDates, zoomToCategoryValues

    this.pricePanel.zoomToIndexes(this.data.length - 100, this.data.length - 1);
  }
}
