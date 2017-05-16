using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{
    /// <summary>
    /// DateTime extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Determines whether [is start of week].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static bool IsStartOfWeek(this DateTime date)
        {            
            return date.DayOfWeek == DayOfWeek.Monday;
        }

        /// <summary>
        /// Determines whether [is start of month].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static bool IsStartOfMonth(this DateTime date)
        {
            return date.Day == 1;
        }

    }
}
