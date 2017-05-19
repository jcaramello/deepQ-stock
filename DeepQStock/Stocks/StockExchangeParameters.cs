using DeepQStock.Enums;
using DeepQStock.Indicators;
using System.Collections.Generic;

namespace DeepQStock.Stocks
{
    public class StockExchangeParameters
    {
        #region << Public Properties >>      

        /// <summary>
        /// Gets or sets the period types.
        /// </summary>
        public PeriodType[] PeriodTypes { get; set; }

        /// <summary>
        /// Gets or sets the length of the episode.
        /// </summary>
        public int EpisodeLength { get; set; }

        /// <summary>
        /// Gets or sets the nro of period.
        /// </summary>
        public int NumberOfPeriods { get; set; }

        /// <summary>
        /// Gets or sets the indicators.
        /// </summary>
        public IList<ITechnicalIndicator> DailyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the weekly indicators.
        /// </summary>
        public IList<ITechnicalIndicator> WeeklyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the monthly indicators.
        /// </summary>
        public IList<ITechnicalIndicator> MonthlyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the transaction cost.
        /// </summary>
        public double TransactionCost { get; set; }

        /// <summary>
        /// Gets or sets the simulation velocity in miliseconds.
        /// </summary>
        public int SimulationVelocity { get; set; }

        /// <summary>
        /// Get or Set the agent initial capital
        /// </summary>
        public double InitialCapital { get; set; }

        /// Gets or sets the in and out strategy that the agent will be use for buy and sell actions.
        /// It should be a value between 0 and 1 that represent the percentage that the agent buy o sell in each transaction.
        /// </summary>
        public double InOutStrategy { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StockExchangeParameters"/> class.
        /// </summary>
        public StockExchangeParameters()
        {
            PeriodTypes = new PeriodType[] { PeriodType.Day, PeriodType.Week, PeriodType.Month };
            EpisodeLength = 7;
            NumberOfPeriods = 14;
            InitialCapital = 100000;
            TransactionCost = 0.01;
            InOutStrategy = 0.25;
            SimulationVelocity = 500;
            DailyIndicators = new List<ITechnicalIndicator>()
            {
                new SimpleMovingAverage(8),
                new ExponentialMovingAverage(20),
                new ExponentialMovingAverage(50),
                new ExponentialMovingAverage(200),
                new AverageTrueRange(),
                new RSI(),
                new DMI(),
                new MACD(),
                new BollingerBandsPercentB()
            };
            WeeklyIndicators = new List<ITechnicalIndicator>()
            {
                new SimpleMovingAverage(8),
                new ExponentialMovingAverage(20),
                new ExponentialMovingAverage(50),
                new ExponentialMovingAverage(200),
                new AverageTrueRange(),
                new RSI(),
                new DMI(),
                new MACD(),
                new BollingerBandsPercentB()
            };
            MonthlyIndicators = new List<ITechnicalIndicator>()
            {
                new SimpleMovingAverage(8),
                new ExponentialMovingAverage(20),
                new ExponentialMovingAverage(50),
                new ExponentialMovingAverage(200),
                new AverageTrueRange(),
                new RSI(),
                new DMI(),
                new MACD(),
                new BollingerBandsPercentB()
            };


        }

        #endregion
    }
}
