using DeepQStock.Domain;
using System.Collections.Generic;

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
