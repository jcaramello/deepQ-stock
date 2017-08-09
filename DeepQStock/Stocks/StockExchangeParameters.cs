using DeepQStock.Enums;
using DeepQStock.Indicators;
using DeepQStock.Storage;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;

namespace DeepQStock.Stocks
{
    public class StockExchangeParameters : BaseModel
    {
        #region << Public Properties >>                                    

        /// <summary>
        /// csv data file path
        /// </summary>
        public string CsvDataFilePath { get; set; }

        /// <summary>
        /// Company Symbol
        /// </summary>
        public string Symbol { get; set; }

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
        /// Gets or sets the reward calculator.
        /// </summary>
        [Ignore]
        public RewardCalculator RewardCalculator { get; set; }

        /// <summary>
        /// Gets or sets the indicators.
        /// </summary>
        [JsonIgnore]
        public IList<TechnicalIndicatorBase> DailyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the weekly indicators.
        /// </summary>
        [JsonIgnore]
        public IList<TechnicalIndicatorBase> WeeklyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the monthly indicators.
        /// </summary>
        [JsonIgnore]
        public IList<TechnicalIndicatorBase> MonthlyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the transaction cost.
        /// </summary>
        public double TransactionCost { get; set; }

        /// <summary>
        /// Current State Id
        /// </summary>
        public long CurrentStateId { get; set; }

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
            RewardCalculator = RewardCalculator.Use(RewardCalculatorType.WinningsOverLoosings);
            DailyIndicators = new List<TechnicalIndicatorBase>()
            {
                new SimpleMovingAverage(PeriodType.Day, 8),
                new ExponentialMovingAverage(PeriodType.Day, 20),
                new ExponentialMovingAverage(PeriodType.Day, 50),
                new ExponentialMovingAverage(PeriodType.Day, 200),
                new AverageTrueRange(PeriodType.Day),
                new RSI(PeriodType.Day),
                new DMI(PeriodType.Day),
                new MACD(PeriodType.Day),
                new BollingerBandsPercentB(PeriodType.Day)
            };
            WeeklyIndicators = new List<TechnicalIndicatorBase>()
            {
                new SimpleMovingAverage(PeriodType.Week, 8),
                new ExponentialMovingAverage(PeriodType.Week, 20),
                new ExponentialMovingAverage(PeriodType.Week, 50),
                new ExponentialMovingAverage(PeriodType.Week, 200),
                new AverageTrueRange(PeriodType.Week),
                new RSI(PeriodType.Week),
                new DMI(PeriodType.Week),
                new MACD(PeriodType.Week),
                new BollingerBandsPercentB(PeriodType.Week)
            };
            MonthlyIndicators = new List<TechnicalIndicatorBase>()
            {
                new SimpleMovingAverage(PeriodType.Month, 8),
                new ExponentialMovingAverage(PeriodType.Month, 20),
                new ExponentialMovingAverage(PeriodType.Month, 50),
                new ExponentialMovingAverage(PeriodType.Month, 200),
                new AverageTrueRange(PeriodType.Month),
                new RSI(PeriodType.Month),
                new DMI(PeriodType.Month),
                new MACD(PeriodType.Month),
                new BollingerBandsPercentB(PeriodType.Month)
            };


        }

        #endregion
    }
}
