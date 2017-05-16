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
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        double[] Value { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        IEnumerable<double> Update(Period period);
    }
}
