using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock
{
    public class State
    {
        #region << Public Properties >>      

        /// <summary>
        /// Gets the current period.
        /// </summary>
        public Period CurrentPeriod
        {
            get
            {
                return Periods.First();
            }
        }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        public IList<Period> Periods { get; set; }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Return a flatted array of all periods
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var flattedPeriods = new List<double>();
            foreach (var p in Periods)
            {
                flattedPeriods.AddRange(p.ToList());
            }

            return flattedPeriods.ToArray();   
        }

        #endregion
    }
}
