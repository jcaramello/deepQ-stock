using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// MACD can be used to identify aspects of a security's overall trend. Most notably these aspects are momentum, as well as trend direction and duration.
    /// What makes MACD so informative is that it is actually the combination of two different types of indicators. 
    /// First, MACD employs two Moving Averages of varying lengths (which are lagging indicators) to identify trend direction and duration
    /// https://www.tradingview.com/wiki/MACD_(Moving_Average_Convergence/Divergence)
    /// </summary>
    public class MACD : ITechnicalIndicator
    {

        #region << Private Properties >>

        /// <summary>
        /// Exponetial moving average of 9 periods
        /// </summary>
        public ExponentialMovingAverage EMA_9 { get; set; }

        /// <summary>
        /// Exponetial moving average of 12 periods
        /// </summary>
        public ExponentialMovingAverage EMA_12 { get; set; }

        /// <summary>
        /// Exponetial moving average of 26 periods
        /// </summary>
        public ExponentialMovingAverage EMA_26 { get; set; }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Calculate(Period period)
        {
            var ema_12 = EMA_12.Calculate(period).First();
            var ema_26 = EMA_12.Calculate(period).First();

            var macd_line = ema_12 - ema_26;
            var signal_line = EMA_9.Calculate(period).First();
            var macd_histogram = macd_line - signal_line;

            return new double[3] { macd_line, signal_line, macd_histogram };
        }

        #endregion
    }
}
