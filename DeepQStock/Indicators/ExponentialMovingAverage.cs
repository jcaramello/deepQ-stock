using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// Exponential Moving Average is very similar to (and is a type of) WMA. The major difference with the EMA is that old data points never leave the average.
    /// To clarify, old data points retain a multiplier (albeit declining to almost nothing) even if they are outside of the selected data series length.
    /// https://www.tradingview.com/wiki/Moving_Average#Exponential_Moving_Average_.28EMA.29
    /// 
    /// There are three steps to calculate the EMA. Here is the formula for a 5 Period EMA
    /// 1. Calculate the SMA
    ///      (Period Values / Number of Periods)
    /// 2. Calculate the Multiplier
    ///      (2 / (Number of Periods + 1)
    /// 3. Calculate the EMA
    /// EMA = { Close - EMA(previous day) } x multiplier + EMA(previous day)
    /// </summary>
    public class ExponentialMovingAverage : SimpleMovingAverage, ITechnicalIndicator
    {
        #region << Private Properties >>        

        /// <summary>
        /// Get the EMA multiplier
        /// </summary>
        private double Multiplier
        {
            get { return (2.0 / (Size + 1.0)); }
        }

        /// <summary>
        /// Mantain Previous EMA
        /// </summary>
        private double PreviousEMA { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="size"></param>
        public ExponentialMovingAverage(int size) : base(size)
        {            
        }

        #endregion

        /// <summary>
        /// Gets the name.
        /// </summary>
        public new string Name { get { return string.Format("EMA({0})", Size); } }

        #region << Overrided Members >>        

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        protected override double AveragePeriods()
        {
            if (Periods.Count < Size)
            {
                return 0.0;
            }

            var period = Periods.Last();

            if (PreviousEMA == 0.0)
            {
                PreviousEMA = base.AveragePeriods();
            }

            PreviousEMA = IndicatorUtils.EMA(period.Close, PreviousEMA, Multiplier);

            return PreviousEMA;
        }

        #endregion
    }
}
