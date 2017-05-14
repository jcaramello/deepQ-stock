using DeepQStock.Enums;
using DeepQStock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Config
{
    public class StockExchangeParameters
    {
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StockExchangeParameters"/> class.
        /// </summary>
        public StockExchangeParameters()
        {
            PeriodTypes = new PeriodType[] { PeriodType.Day, PeriodType.Week, PeriodType.Month };
            EpisodeLength = 7;
            NumberOfPeriods = 14;
            TransactionCost = 0.01;
            Indicators = new List<ITechnicalIndicator>()
            {
                new SimpleMovingAverage(8),
                new ExponentialMovingAverage(20),
                new ExponentialMovingAverage(50),
                new ExponentialMovingAverage(200),
                new RSI(),
                new DMI()
            };
        }

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        public IAgent Agent { get; set; }

        /// <summary>
        /// Gets or sets the CSV file path.
        /// </summary>
        public string CsvFilePath { get; set; }

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
        public IList<ITechnicalIndicator> Indicators { get; set; }

        /// <summary>
        /// Gets or sets the transaction cost.
        /// </summary>
        public double TransactionCost { get; set; }

        /// <summary>
        /// Gets or sets the simulation velocity.
        /// </summary>
        public int? SimulationVelocity { get; set; }

        /// <summary>
        /// Get or Set the agent initial capital
        /// </summary>
        public double InitialCapital { get; set; }

        #endregion
    }
}
