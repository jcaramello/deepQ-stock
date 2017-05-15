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

        /// <summary>
        /// Calculate an standart deviation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StandardDeviation(IEnumerable<double> values)
        {
            double std_dev = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(v => Math.Pow(v - avg, 2));

                //Put it all together
                std_dev = Math.Sqrt(sum / count);
            }
            return std_dev;
        }
    }
}
