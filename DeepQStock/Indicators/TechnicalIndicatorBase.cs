using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// Base Class for technical indicators
    /// </summary>
    public class TechnicalIndicatorBase : BaseModel, ITechnicalIndicator
    {
        public string ClassType { get; set; }

        /// <summary>
        /// Stock Exchange id
        /// </summary>
        [ForeignKey(typeof(StockExchangeParameters))]
        public long StockExchangeId { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [TextBlob("Value")]
        public double[] Value { get; set; }

        /// <summary>
        /// Indicator Type
        /// </summary>
        public PeriodType Type { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name { get; }


        public TechnicalIndicatorBase(PeriodType type)
        {
            ClassType = GetType().FullName;
            Type = type;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<double> Update(Period period, bool normalize = true)
        {
            return null;
        }
    }
}
