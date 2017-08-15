using DeepQStock.Domain;
using System;
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
        Period NextDay();

        /// <summary>
        /// Get the next the period.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<Period> NextDays();

        /// <summary>
        /// Return all the data
        /// </summary>
        /// <returns></returns>
        IEnumerable<Period> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        void Seek(DateTime? startDate = null, DateTime? endDate = null);
    }
}
