using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{
    /// <summary>
    /// The Average True Range (ATR) is a tool used in technical analysis to measure volatility. 
    /// Unlike many of today's popular indicators, the ATR is not used to indicate the direction of price. 
    /// Rather, it is a metric used solely to measure volatility, especially volatility caused by price gaps or limit moves.
    /// https://www.tradingview.com/wiki/Average_True_Range_(ATR)
    /// </summary>
    public class AverageTrueRange : TechnicalIndicatorBase,  ITechnicalIndicator
    {        
        #region << Properties >>

        /// <summary>
        /// Total of periods used
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Get the EMA multiplier
        /// </summary>
        private double Multiplier
        {
            get { return (2.0 / (Length + 1.0)); }
        }

        /// <summary>
        /// Get or sets the previous atr
        /// </summary>
        public double PreviousATR { get; set; }

        /// <summary>
        /// Get or set the previous period
        /// </summary>
        public Period PreviousPeriod { get; set; }


        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="length"></param>
        public AverageTrueRange(PeriodType type, int length = 14) : base(type)
        {
            Length = length;           
        }

        #endregion

        #region  IStockExchangeIndicators Members >>

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get { return "ATR"; } }            

        /// <summary>
        /// Calculate the average true range
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override IEnumerable<double> Update(Period period, bool normalize = true)
        {
            if (PreviousPeriod != null)
            {
                var values = new double[3]
                {
                    period.High - period.Low,
                    Math.Abs(period.High - PreviousPeriod.Close),
                    Math.Abs(period.Low - PreviousPeriod.Close)
                };

                var tr = values.Max();
               
                PreviousATR = (tr - PreviousATR) * Multiplier + PreviousATR;                
            }

            PreviousPeriod = period;

            Value = new double[1] { PreviousATR };

            return normalize ? Value.Select(v => Normalizers.Price.Normalize(v)) : Value;
        }

        #endregion
    }
}
