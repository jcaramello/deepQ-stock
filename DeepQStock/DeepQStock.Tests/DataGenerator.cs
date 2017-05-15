using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Tests
{
    public static class DataGenerator
    {
        /// <summary>
        /// Gets the sample periods.
        /// </summary>
        /// <returns></returns>
        public static IList<Period> GetSamplePeriods()
        {
            return new List<Period>()
            {
                new Period() { Close = 22.27 },
                new Period() { Close = 22.19 },
                new Period() { Close = 22.08 },
                new Period() { Close = 22.17 },
                new Period() { Close = 22.18 },
                new Period() { Close = 22.13 },
                new Period() { Close = 22.23 },
                new Period() { Close = 22.43 },
                new Period() { Close = 22.24 },
                new Period() { Close = 22.29 },
                new Period() { Close = 22.15 }
            };
        }
    }
}
