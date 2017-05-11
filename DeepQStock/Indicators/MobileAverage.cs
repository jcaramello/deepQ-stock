using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace deepQStock.Indicators
{
    public class MobileAverage : IStockExchangeIndicator
    {
        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<float> GetValue(Period period)
        {
            return null;
        }

        #endregion
    }
}
