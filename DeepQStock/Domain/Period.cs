using DeepQStock.Enums;
using DeepQStock.Indicators;
using DeepQStock.Storage;
using DeepQStock.Utils;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Data.Entity;

namespace DeepQStock.Domain
{
    public class Period : BaseModel
    {
        #region << Public Properties >>            

        /// <summary>
        /// Period Type
        /// </summary>
        public PeriodType PeriodType { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>        
        [CsvColumn(Name = "date", FieldIndex = 1)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the close price.
        /// </summary>       
        [CsvColumn(Name = "close", FieldIndex = 2)]
        public double Close { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>       
        [CsvColumn(Name = "volume", FieldIndex = 3)]
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the open price.
        /// </summary>       
        [CsvColumn(Name = "open", FieldIndex = 4)]
        public double Open { get; set; }

        /// <summary>
        /// Gets or sets the high price.
        /// </summary>      
        [CsvColumn(Name = "high", FieldIndex = 5)]
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low price.
        /// </summary>        
        [CsvColumn(Name = "low", FieldIndex = 6)]
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the current capital.
        /// </summary>
        public double CurrentCapital { get; set; }

        /// <summary>
        /// Gets or sets the actual posicion.
        /// </summary>
        public int ActualPosition { get; set; }

        /// <summary>
        /// Gets or sets the indicators.
        /// </summary>            
        [NotMapped]
        public virtual IReadOnlyCollection<IndicatorValue> Indicators
        {
            get
            {                
                if (Id > 0 && InternalIndicators.Count == 0)
                {
                    using (var ctx = new DeepQStockContext())
                    {
                        ctx.Configuration.LazyLoadingEnabled = false;
                        ctx.Configuration.ProxyCreationEnabled = false;

                        InternalIndicators = ctx.Periods.Include(p => p.InternalIndicators).Single(p => p.Id == Id).InternalIndicators;                        
                    }
                }

                return InternalIndicators.ToList().AsReadOnly();
            }
        }

        public ICollection<IndicatorValue> InternalIndicators { get; set; }

        /// <summary>
        /// States
        /// </summary>
        public ICollection<State> States { get; set; }

        /// <summary>
        /// Averages
        /// </summary>
        public ICollection<SimpleMovingAverage> Averages { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Period()
        {            
            InternalIndicators = new List<IndicatorValue>();
            PeriodType = PeriodType.Day;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Merges the specified period.
        /// </summary>
        /// <param name="period">The period.</param>
        public void Merge(Period period)
        {
            Close = period.Close;
            High = Math.Max(High, period.High);
            Low = Math.Min(Low, period.Low);
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public IList<double> ToList()
        {
            var period = new List<double>
            {
                Normalizers.Capital.Normalize(CurrentCapital),
                Normalizers.Position.Normalize(ActualPosition),
                Normalizers.Price.Normalize(Open),
                Normalizers.Price.Normalize(Close),
                Normalizers.Price.Normalize(High),
                Normalizers.Price.Normalize(Low),
                Normalizers.Volume.Normalize(Volume)
            };           

            foreach (var i in Indicators)
            {
                period.AddRange(i.Values);
            }

            return period;
        }

        /// <summary>
        /// Adds the indicator.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddIndicator(IndicatorValue value)
        {
            InternalIndicators.Add(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var values = Indicators.Select(p => string.Format("{0}:[{1}]", p.Name, string.Join(",", p.Values.Select(v => v.ToString("N4", CultureInfo.InvariantCulture)))));
            return string.Join(" | ", values);
        }

        #endregion

        #region <<  ICloneable Members >>

        public Period Clone()
        {
            return new Period()
            {
                ActualPosition = ActualPosition,
                CurrentCapital = CurrentCapital,
                Date = Date,
                Open = Open,
                Close = Close,
                High = High,
                Low = Low,
                Volume = Volume
            };

        }

        #endregion
    }
}
