using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// The Average True Range (ATR) is a tool used in technical analysis to measure volatility. 
    /// Unlike many of today's popular indicators, the ATR is not used to indicate the direction of price. 
    /// Rather, it is a metric used solely to measure volatility, especially volatility caused by price gaps or limit moves.
    /// https://www.tradingview.com/wiki/Average_True_Range_(ATR)
    /// </summary>
    public class AverageTrueRange : ITechnicalIndicator
    {
        #region << Private Properties >>

        /// <summary>
        /// Total of periods used
        /// </summary>
        private int Length { get; set; }

        /// <summary>
        /// Get the EMA multiplier
        /// </summary>
        private double Multiplier
        {
            get { return (2 / (Length + 1)); }
        }

        /// <summary>
        /// Get or sets the previous atr
        /// </summary>
        private double PreviousATR { get; set; }

        /// <summary>
        /// Get or set the previous period
        /// </summary>
        private Period PreviousPeriod { get; set; }


        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="length"></param>
        public AverageTrueRange(int length = 14)
        {
            Length = length;           
        }

        #endregion

        #region  IStockExchangeIndicators Members >>

        /// <summary>
        /// Calculate the average true range
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IEnumerable<double> Calculate(Period period)
        {
            if (PreviousPeriod != null)
            {
                var values = new double[3]
                {
                    period.High - period.Low,
                    Math.Abs(period.High - PreviousPeriod.Close),
                    Math.Abs(period.Low - PreviousPeriod.Close)
                };

                var tr = values.Max();
               
                PreviousATR = (tr - PreviousATR) * Multiplier + PreviousATR;                
            }

            PreviousPeriod = period;

            return new double[1] { PreviousATR };
        }

        #endregion
    }
}
