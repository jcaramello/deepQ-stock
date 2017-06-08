using DeepQStock.Enums;
using DeepQStock.Indicators;
using DeepQStock.Storage;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DeepQStock.Stocks
{
    public class StockExchangeParameters : BaseModel
    {
        #region << Public Properties >>             

        /// <summary>
        /// Gets or sets the period types.
        /// </summary>
        [JsonIgnore]
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
        [JsonIgnore]
        public IList<ITechnicalIndicator> DailyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the weekly indicators.
        /// </summary>
        [JsonIgnore]
        public IList<ITechnicalIndicator> WeeklyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the monthly indicators.
        /// </summary>
        [JsonIgnore]
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
            SimulationVelocity = 0;
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
