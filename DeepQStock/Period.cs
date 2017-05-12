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
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>        
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the current capital.
        /// </summary>
        public double CurrentCapital { get; set; }

        /// <summary>
        /// Gets or sets the actual posicion.
        /// </summary>
        public int ActualPosicion { get; set; }

        /// <summary>
        /// Gets or sets the open price.
        /// </summary>       
        public double OpenPrice { get; set; }

        /// <summary>
        /// Gets or sets the close price.
        /// </summary>       
        public double ClosePrice { get; set; }

        /// <summary>
        /// Gets or sets the high price.
        /// </summary>      
        public double HighPrice { get; set; }

        /// <summary>
        /// Gets or sets the low price.
        /// </summary>        
        public double LowPrice { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>       
        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the indicators.
        /// </summary>
        public IList<IStockExchangeIndicator> Indicators { get; set; }


        #endregion

        #region << Public Methods >>

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public IList<double> ToList()
        {
            return null;     
        }

        #endregion
    }
}
