using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock
{
    public interface IDataProvider
    {
        /// <summary>
        /// Get the next the period.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<Period> NextDays();
    }
}
