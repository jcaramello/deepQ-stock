using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public long StockExchangeId { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>        
        [NotMapped]
        public double[] Value
        {
            get
            {
              return Array.ConvertAll(InternalValue.Split(';'), Double.Parse);
            }
            set
            {
                
                InternalValue = String.Join(";", value.Select(p => p.ToString()).ToArray());
            }
        }

        /// <summary>
        /// Gets or sets the internal value.
        /// </summary>       
        public string InternalValue { get; set; }

        /// <summary>
        /// Indicator Type
        /// </summary>
        public PeriodType Type { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name { get; }


        public TechnicalIndicatorBase()
        {
            Type = PeriodType.Day;
        }

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
