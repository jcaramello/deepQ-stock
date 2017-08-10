using DeepQStock.Enums;
using DeepQStock.Storage;
using DeepQStock.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Domain
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
    public class State : BaseModel
    {
        #region << Public Properties >>     

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the stock exchange identifier.
        /// </summary>
        public long StockExchangeId { get; set; }

        /// <summary>
        /// Gets the current period.
        /// </summary>
        [NotMapped]       
        public Period Today
        {
            get
            {
                return DayLayer.Last();
            }
        }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        [NotMapped]
        public CircularQueue<Period> DayLayer { get; set; }

        /// <summary>
        /// Gets or sets the week layer.
        /// </summary>
        [NotMapped]
        public CircularQueue<Period> WeekLayer { get; set; }

        /// <summary>
        /// Gets or sets the month layer.
        /// </summary>
        [NotMapped]
        public CircularQueue<Period> MonthLayer { get; set; }

        /// <summary>
        /// Gets or sets the period ids.
        /// </summary>                
        public ICollection<Period> FlattenPeriods
        {
            get
            {
                return DayLayer.Concat(WeekLayer).Concat(MonthLayer).ToList();
            }
            set
            {
                InitializeLayer(DayLayer, value.Where(p => p.PeriodType == PeriodType.Day));
                InitializeLayer(WeekLayer, value.Where(p => p.PeriodType == PeriodType.Week));
                InitializeLayer(MonthLayer, value.Where(p => p.PeriodType == PeriodType.Month));
            }
        }

        #endregion

        #region << Constructor >> 

        public State() : this(14){ }

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

            clone.Id = Id;
            DayLayer?.ToList().ForEach(p => clone.DayLayer.Enqueue(p));
            WeekLayer?.ToList().ForEach(p => clone.WeekLayer.Enqueue(p));
            MonthLayer?.ToList().ForEach(p => clone.MonthLayer.Enqueue(p));            

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

        /// <summary>
        /// Initializes the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="periods">The periods.</param>
        private void InitializeLayer(CircularQueue<Period> layer, IEnumerable<Period> periods)
        {
            foreach (var p in periods.OrderBy(p => p.Date))
            {
                layer.Enqueue(p);
            }
        }

        #endregion
    }
}
