using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// The Relative Strength Index (RSI) is a well versed momentum based oscillator which is used to measure the speed (velocity) 
    /// as well as the change (magnitude) of directional price movements. Essentially RSI, when graphed, 
    /// provides a visual mean to monitor both the current, as well as historical, strength and weakness of a particular market.
    /// The strength or weakness is based on closing prices over the duration of a specified trading period creating a reliable 
    /// metric of price and momentum changes. Given the popularity of cash settled instruments (stock indexes) 
    /// and leveraged financial products (the entire field of derivatives); RSI has proven to be a viable indicator of price movements.
    /// https://www.tradingview.com/wiki/Relative_Strength_Index_(RSI)
    /// </summary>
    public class RSI : ITechnicalIndicator
    {
        #region << Private Properties >>

        /// <summary>
        /// The number of periods to use for calculate the rsi
        /// </summary>
        private int Length { get; set; }

        /// <summary>
        /// Upward Periods used in the calculation
        /// </summary>
        private ExponentialMovingAverage UpwardPeriods{ get; set; }

        /// <summary>
        /// Upward Periods used in the calculation
        /// </summary>
        private ExponentialMovingAverage DownwardPeriods { get; set; }

        /// <summary>
        /// Previous Period
        /// </summary>
        private Period PreviousPeriod{ get; set; }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="size"></param>
        public RSI(int length = 14)
        {
            Length = length;
            UpwardPeriods = new ExponentialMovingAverage(Length);
            DownwardPeriods = new ExponentialMovingAverage(Length);
        }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Calculate(Period period)
        {
            double emaU = 0;
            double emaD = 0;
            double rsi = 0;

            if (PreviousPeriod != null && (Length >= UpwardPeriods.Periods.Count + DownwardPeriods.Periods.Count))
            {
                if (PreviousPeriod.Close > period.Close)
                {
                    emaU = UpwardPeriods.Calculate(period).First();
                }
                else
                {
                    emaU = DownwardPeriods.Calculate(period).First();
                }

                var rs = emaU / emaD;
                rsi = 100 - (100 / 1 + rs);
            }

            PreviousPeriod = period;

            return new double[1] { rsi };
        }

        #endregion
    }
}
