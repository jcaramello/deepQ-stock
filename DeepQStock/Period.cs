using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock
{
    public class Period
    {
        #region << Public Properties >> 

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
        public int Volume { get; set; }

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
        public int ActualPosicion { get; set; }                          

        /// <summary>
        /// Gets or sets the indicators.
        /// </summary>
        public List<double> Indicators { get; set; }


        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Period()
        {
            Indicators = new List<double>();
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public IList<double> ToList()
        {
            var period = new List<double> { CurrentCapital, ActualPosicion, Open, Close, Volume, High, Low};

            return period.Concat(Indicators).ToList();
        }

        #endregion
    }
}
