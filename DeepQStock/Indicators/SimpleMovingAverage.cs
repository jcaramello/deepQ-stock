using DeepQStock.Domain;
using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// Moving Average (MA) is a price based, lagging (or reactive) indicator that displays the average price of a security over a set period of time.
    /// A Moving Average is a good way to gauge momentum as well as to confirm trends, and define areas of support and resistance. Essentially, 
    /// Moving Averages smooth out the “noise” when trying to interpret charts. Noise is made up of fluctuations of both price and volume. 
    /// https://www.tradingview.com/wiki/Moving_Average
    /// </summary>
    public class SimpleMovingAverage : TechnicalIndicatorBase, ITechnicalIndicator
    {       
        #region << Public Properties >>

        /// <summary>
        /// List of Periods to use for calculate the average
        /// </summary>
        public Queue<Period> Periods { get; set; }

        /// <summary>
        /// Set or Get the size of the mobile average
        /// </summary>
        public int Size { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="size">The number of periods to consider</param>
        public SimpleMovingAverage(PeriodType type, int size) : base(type)
        {
            Size = size;            
            Periods = new Queue<Period>(Size);
        }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get { return string.Format("SMA({0})", Size);  } }      

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<double> Update(Period period)
        {
            if (Periods.Count == Size)
            {
                Periods.Dequeue();
            }

            Periods.Enqueue(period);

            Value =  new double[1] { AveragePeriods() };
            return Value;
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Calculate the the average of the current periods
        /// </summary>
        /// <returns></returns>
        protected virtual double AveragePeriods()
        {
            return Periods.Average(p => p.Close);
        }

        #endregion
    }
}
