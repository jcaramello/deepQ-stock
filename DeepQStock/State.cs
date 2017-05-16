using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock
{
    /// <summary>
    /// Represent the enviroment state.
    /// We can think it, as cube of 3 levels, each level is a matrix of periods and
    /// each level have a pre-defined size.
    /// 
    /// The structure of each layer is the following:
    ///   - The first level is a matrix of day's periods, 
    ///   - The second level is a matrix of week's periods
    ///   - And finally the thrid layer is a matrix of month's periods
    /// 
    /// </summary>
    public class State 
    {
        #region << Public Properties >>      

        /// <summary>
        /// Gets the current period.
        /// </summary>
        public Period Today
        {
            get
            {
                return DayLayer.Peek();
            }
        }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        public CircularQueue<Period> DayLayer { get; set; }

        /// <summary>
        /// Gets or sets the week layer.
        /// </summary>
        public CircularQueue<Period> WeekLayer { get; set; }

        /// <summary>
        /// Gets or sets the month layer.
        /// </summary>
        public CircularQueue<Period> MonthLayer { get; set; }

        #endregion

        #region << Private Properties >> 

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; set; }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State(int size = 14)
        {
            Size = size;
            DayLayer = new CircularQueue<Period>(size);
            WeekLayer = new CircularQueue<Period>(size);
            MonthLayer = new CircularQueue<Period>(size);
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Return a flatted array of all periods in all layers
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var flattedPeriods = new List<double>();

            flattedPeriods.AddRange(FlattenLayer(DayLayer));
            flattedPeriods.AddRange(FlattenLayer(WeekLayer));
            flattedPeriods.AddRange(FlattenLayer(MonthLayer));

            return flattedPeriods.ToArray();   
        }

        #endregion

        #region << ICloneable Members >>

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public State Clone()
        {
            var clone = new State(Size);

            DayLayer.ToList().ForEach(p => clone.DayLayer.Enqueue(p));
            WeekLayer.ToList().ForEach(p => clone.WeekLayer.Enqueue(p));
            MonthLayer.ToList().ForEach(p => clone.MonthLayer.Enqueue(p));

            return clone;
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Flattens the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns></returns>
        private IEnumerable<double> FlattenLayer(CircularQueue<Period> layer)
        {
            var flattedPeriods = new List<double>();

            foreach (var period in layer)
            {
                flattedPeriods.AddRange(period.ToList());
            }

            var missingPeriods = Size - layer.Count;

            if (missingPeriods > 0)
            {
                flattedPeriods.AddRange(new double[missingPeriods]);
            }

            return flattedPeriods;
        }

        #endregion
    }
}
