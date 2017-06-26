using DeepQStock.Enums;
using DeepQStock.Storage;
using LINQtoCSV;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Domain
{
    public class Period : BaseModel
    {
        #region << Public Properties >>            

        /// <summary>
        /// Period Type
        /// </summary>
        public PeriodType  PeriodType{ get; set; }

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
        public IDictionary<string, IEnumerable<double>> Indicators { get; set; }      

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Period()
        {
            Indicators = new Dictionary<string, IEnumerable<double>>();
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
                CurrentCapital,
                ActualPosition,
                Open,
                Close,                
                High,
                Low,
                Volume
            };

            foreach (var pair in Indicators)
            {
                period.AddRange(pair.Value.Select(v => Math.Round(v, 4)));
            }

            return period;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var values = Indicators.Select(p => string.Format("{0}:[{1}]", p.Key, string.Join(",", p.Value.Select(v => v.ToString("N4", CultureInfo.InvariantCulture)))));
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
