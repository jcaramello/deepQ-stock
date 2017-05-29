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

    AmCharts.ready(this.init.bind(this));

  }

 

  /**
   * Initialize the charts
   * 
   * @private
   * 
   * @memberof CandleStickComponent
   */
  private init() {

    var symbol = "MSFT";

    var chart = this.chart = new AmCharts.AmStockChart();
    chart.theme = 'light';
    chart.pathToImages = "/bower_components/amcharts3/amcharts/images/";
    chart['dataDateFormat'] = "YYYY/MM/DD";
    chart.valueAxesSettings.position = 'right';
    chart.valueAxesSettings.inside = false;
    chart.categoryAxesSettings.equalSpacing = true;
    //chart.categoryAxesSettings.groupToPeriods = ["DD", "WW"];

    var dataSet = new AmCharts.DataSet();
    //dataSet.dataProvider = this.data;
    var loader = {
      url: "assets/data/" + symbol + ".csv",
      format: "csv",
      showCurtain: true,
      showErrors: true,
      async: true,
      reverse: true,
      delimiter: ",",
      useColumnNames: true
    };

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

    var legend = new AmCharts.StockLegend();
    pricePanel.stockLegend = legend;

    var panelsSettings = new AmCharts.PanelsSettings();
    panelsSettings.startDuration = 1;

    chart.panelsSettings = panelsSettings;

    var priceGraph = new AmCharts.StockGraph();
    priceGraph.valueField = "close";
    priceGraph.type = "candlestick";
    priceGraph.lowField = 'low';
    priceGraph.highField = 'high';
    priceGraph.openField = 'open';
    priceGraph.closeField = 'close';
    priceGraph.balloonText = "Open:<b>[[open]]</b><br>Low:<b>[[low]]</b><br>High:<b>[[high]]</b><br>Close:<b>[[close]]</b><br>";
    priceGraph.title = symbol;
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
    volGraph.fillColors = '#ffeb99';
    volGraph.useDataSetColors = false;
    volGraph.fillAlphas = 1
    volGraph.compareField = 'volume';
    volGraph.fillColors = "#ffeb99";
    volGraph.lineColor = '#ffeb99';

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
    sbsettings.autoGridCount = true;
    sbsettings.updateOnReleaseOnly = false;
    sbsettings.graphType = 'line';
    chart.chartScrollbarSettings = sbsettings;

    chart.chartCursorSettings.bulletsEnabled = true;
    chart.chartCursorSettings.valueBalloonsEnabled = true;
    chart.chartCursorSettings.cursorColor = "#8c8c8c"
    chart.chartCursorSettings.zoomable = true;

    var periodSelector = new AmCharts.PeriodSelector();
    periodSelector.periods = [
      { period: "MM", count: 1, label: "1M", selected: true },
      { period: "MM", count: 6, label: "6M" },
      { period: "YYYY", count: 1, label: "1Y" },
      { period: "YYYY", count: 5, label: "5Y" }
    ];

    chart.periodSelector = periodSelector;

    chart.panels = [pricePanel, volPanel];
    chart.addListener("rendered", this.zoomChart.bind(this));
    chart.write('candlestick-' + this.id);
    this.zoomChart();
  }

  // this method is called when chart is first inited as we listen for "rendered" event
  private zoomChart() {
    // different zoom methods can be used - zoomToIndexes, zoomToDates, zoomToCategoryValues

    this.pricePanel.zoomToIndexes(this.data.length - 100, this.data.length - 1);
  }
}
