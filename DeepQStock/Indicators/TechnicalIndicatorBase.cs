using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

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
        public string InternalValue
        {
            get
            {
                return Value != null ? string.Join(";", Value.Select(p => p.ToString())) : null;
            }
            set
            {
                Value = value != null ? Array.ConvertAll(value.Split(';'), double.Parse) : null;
            }
        }

        /// <summary>
        /// Gets or sets the internal value.
        /// </summary>       
        public double[] Value { get; set; }

        /// <summary>
        /// Indicator Type
        /// </summary>
        public PeriodType Type { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name { get; }

        public TechnicalIndicatorBase() : this(PeriodType.Day, 0) { }

        public TechnicalIndicatorBase(PeriodType type = PeriodType.Day, long stockExchangeId = 0)
        {
            ClassType = GetType().FullName;
            Type = type;
            StockExchangeId = stockExchangeId;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<double> Update(Period period, bool normalize = true)
        {
            return null;
        }

        /// <summary>
        /// Save the indicator
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void Save(DeepQStockContext ctx)
        {
            var set = ctx.Set(System.Type.GetType(ClassType));
            var dbObj = set.Find(Id);

            if (dbObj == null)
            {
                set.Add(this);
            }
            else
            {
                ctx.Entry(dbObj).CurrentValues.SetValues(this);
                ctx.Entry(dbObj).State = EntityState.Modified;
            }            
        }
    }
}
