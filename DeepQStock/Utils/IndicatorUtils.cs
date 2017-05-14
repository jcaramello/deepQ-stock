using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{
    public static class IndicatorUtils
    {
        /// <summary>
        /// Calculate a EMA
        /// </summary>
        /// <param name="val"></param>
        /// <param name="previousEMA"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static double EMA(double val, double previousEMA, double multiplier)
        {
            return (val - previousEMA) * multiplier + previousEMA;
        }
    }
}
