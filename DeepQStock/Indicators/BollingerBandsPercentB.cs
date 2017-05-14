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
        
        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Calculate(Period period)
        {
            return null;
        }

        #endregion
    }
}
