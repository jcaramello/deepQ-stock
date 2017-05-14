using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    public interface ITechnicalIndicator
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        IEnumerable<double> Calculate(Period period);
    }
}
