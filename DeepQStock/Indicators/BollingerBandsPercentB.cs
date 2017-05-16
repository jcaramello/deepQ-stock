﻿using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// Bollinger Bands are a volatility indicator which creates a band of three lines which are plotted in relation to a security's price. 
    /// The Middle Line is typically a 20 Day Simple Moving Average. The Upper and Lower Bands are typically 2 standard deviations above and below the SMA (Middle Line). 
    /// What the %B indicator does is quantify or display where price is in relation to the bands. %B can be useful in identifying trends and trading signals.
    /// https://www.tradingview.com/wiki/Bollinger_Bands_%25B_(%25B)
    /// </summary>
    public class BollingerBandsPercentB : ITechnicalIndicator
    {
        #region << Private Properties >>

        /// <summary>
        /// Simple moving average of 20 periods
        /// </summary>
        public SimpleMovingAverage MA_20 { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BollingerBandsPercentB()
        {
            MA_20 = new SimpleMovingAverage(20);
        }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get { return "Bollinger Bands %B"; } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double[] Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Update(Period period)
        {
            var ma_20 = MA_20.Update(period).First();
            var two_std_dev = 2 * IndicatorUtils.StandardDeviation(MA_20.Periods.Select(p => p.Close));

            var upperBand = period.Close + two_std_dev;
            var lowerBand = period.Close - two_std_dev;

            Value = new double[3] { upperBand, ma_20, lowerBand };
            return Value;
        }

        #endregion
    }
}
