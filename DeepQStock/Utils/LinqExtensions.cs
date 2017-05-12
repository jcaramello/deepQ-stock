using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Maximums the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="en">The en.</param>
        /// <param name="evaluate">The evaluate.</param>
        /// <returns></returns>
        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

    }
}
