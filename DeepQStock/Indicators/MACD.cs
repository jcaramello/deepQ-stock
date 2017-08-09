using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Utils;
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
    public class MACD : TechnicalIndicatorBase, ITechnicalIndicator
    {
        #region << Properties >>

        /// <summary>
        /// Exponetial moving average of 9 periods
        /// </summary>        
        public ExponentialMovingAverage Ema9 { get; set; }
       
        /// <summary>
        /// Exponetial moving average of 12 periods
        /// </summary>        
        public ExponentialMovingAverage Ema12 { get; set; }        

        /// <summary>
        /// Exponetial moving average of 26 periods
        /// </summary>        
        public ExponentialMovingAverage Ema26 { get; set; }
      
        #endregion

        #region << Constructor >>

        public MACD() : base(PeriodType.Day)
        {
            Ema9 = new ExponentialMovingAverage(Type, 9);
            Ema12 = new ExponentialMovingAverage(Type, 12);
            Ema26 = new ExponentialMovingAverage(Type, 26);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MACD"/> class.
        /// </summary>
        public MACD(PeriodType type) : base(type)
        {
            Ema9 = new ExponentialMovingAverage(type, 9);
            Ema12 = new ExponentialMovingAverage(type, 12);
            Ema26 = new ExponentialMovingAverage(type, 26);
        }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get { return "MACD"; } }
      
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<double> Update(Period period, bool normalize = true)
        {
            var ema_12 = Ema12.Update(period, false).First();
            var ema_26 = Ema12.Update(period, false).First();

            var macd_line = ema_12 - ema_26;
            var signal_line = Ema9.Update(period, false).First();
            var macd_histogram = macd_line - signal_line;

            Value = new double[3] { macd_line, signal_line, macd_histogram };

            return normalize ? Value.Select(v => Normalizers.Price.Normalize(v)) : Value;
        }

        #endregion
    }
}
