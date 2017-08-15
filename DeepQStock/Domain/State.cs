using DeepQStock.Enums;
using DeepQStock.Indicators;
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
                return DayLayer.LastOrDefault();
            }
        }

        /// <summary>
        /// Gets the current period.
        /// </summary>
        [NotMapped]
        public Period Yesterday
        {
            get
            {
                return DayLayer.Count > 2 ? DayLayer.ElementAt(DayLayer.Count - 2) : null;
            }
        }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        [NotMapped]
        private CircularQueue<Period> DayLayer
        {
            get
            {
                if (_dayLayer.Count == 0)
                {
                    InitializeLayer(_dayLayer, InternalPeriods.Where(p => p.PeriodType == PeriodType.Day));
                }

                return _dayLayer;
            }
            set
            {
                _dayLayer = value;
            }
        }

        /// <summary>
        /// Gets or sets the week layer.
        /// </summary>
        [NotMapped]
        private CircularQueue<Period> WeekLayer
        {
            get
            {
                if (_weekLayer.Count == 0)
                {
                    InitializeLayer(_weekLayer, InternalPeriods.Where(p => p.PeriodType == PeriodType.Week));
                }

                return _weekLayer;
            }
            set
            {
                _weekLayer = value;
            }
        }

        /// <summary>
        /// Gets or sets the month layer.
        /// </summary>
        [NotMapped]
        private CircularQueue<Period> MonthLayer
        {
            get
            {
                if (_monthLayer.Count == 0)
                {
                    InitializeLayer(_monthLayer, InternalPeriods.Where(p => p.PeriodType == PeriodType.Month));
                }

                return _monthLayer;
            }
            set
            {
                _monthLayer = value;
            }
        }

        /// <summary>
        /// Internal Periods
        /// </summary>
        public virtual ICollection<Period> InternalPeriods { get; set; }

        [NotMapped]
        private CircularQueue<Period> _dayLayer;

        [NotMapped]
        private CircularQueue<Period> _weekLayer;

        [NotMapped]
        private CircularQueue<Period> _monthLayer;

        #endregion

        #region << Constructor >> 

        public State() : this(14) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State(int size = 14)
        {
            Size = size;
            DayLayer = new CircularQueue<Period>(size);
            WeekLayer = new CircularQueue<Period>(size);
            MonthLayer = new CircularQueue<Period>(size);
            _dayLayer = new CircularQueue<Period>(Size);
            _weekLayer = new CircularQueue<Period>(Size);
            _monthLayer = new CircularQueue<Period>(Size);
            InternalPeriods = new List<Period>();
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="upcomingDay"></param>
        /// <param name="dailyIndicators"></param>
        /// <param name="weeklyIndicators"></param>
        /// <param name="monthlyIndicators"></param>
        public void UpdateLayers(Period upcomingDay, IEnumerable<ITechnicalIndicator> dailyIndicators, IEnumerable<ITechnicalIndicator> weeklyIndicators, IEnumerable<ITechnicalIndicator> monthlyIndicators)
        {
            foreach (var indicator in dailyIndicators)
            {
                var values = indicator.Update(upcomingDay);
                upcomingDay.AddIndicator(new IndicatorValue(indicator.Name, values));
            }

            DayLayer.Enqueue(upcomingDay);

            UpdateLayer(PeriodType.Day, DayLayer, upcomingDay, dailyIndicators);
            UpdateLayer(PeriodType.Week, WeekLayer, upcomingDay, weeklyIndicators);
            UpdateLayer(PeriodType.Month, MonthLayer, upcomingDay, monthlyIndicators);

            InternalPeriods = DayLayer.Concat(WeekLayer).Concat(MonthLayer).ToList();
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

            InternalPeriods?.ToList().ForEach(p => clone.InternalPeriods.Add(p));

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

        /// <summary>
        /// Updates a layer of the state.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="upcomingDay">The upcoming day.</param>
        /// <param name="Indicators">The indicators.</param>
        private void UpdateLayer(PeriodType type, CircularQueue<Period> layer, Period upcomingDay, IEnumerable<ITechnicalIndicator> indicators)
        {
            Period currentPeriod = null;
            bool needNewPeriod = type == PeriodType.Week ? upcomingDay.Date.IsStartOfWeek() : upcomingDay.Date.IsStartOfMonth();

            if (layer.IsEmpty || needNewPeriod)
            {
                currentPeriod = upcomingDay.Clone();
                currentPeriod.PeriodType = type;
                layer.Enqueue(currentPeriod);
            }
            else
            {
                currentPeriod = layer.Peek();
                currentPeriod.Merge(upcomingDay);
            }

            foreach (var indicator in indicators)
            {
                var newValues = indicator.Update(currentPeriod);
                var periodIndicator = currentPeriod.Indicators.SingleOrDefault(i => i.Name == indicator.Name);
                if (periodIndicator != null)
                {
                    periodIndicator.Values = newValues;
                }
                else
                {
                    currentPeriod.AddIndicator(new IndicatorValue(indicator.Name, newValues));
                }
            }
        }

        #endregion
    }
}
