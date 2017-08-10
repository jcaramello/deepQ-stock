using DeepQStock.Enums;
using DeepQStock.Indicators;
using DeepQStock.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
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
        [NotMapped]
        public RewardCalculator RewardCalculator { get; set; }

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
            RewardCalculator = RewardCalculator.Use(RewardCalculatorType.WinningsOverLoosings);         
        }

        #endregion
    }
}
