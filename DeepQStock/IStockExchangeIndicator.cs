using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock
{
    public interface IStockExchangeIndicator
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        IEnumerable<float> GetValue(Period period);
    }
}
