using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Storage;
using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Indicators
{

    /// <summary>
    /// Directional Movement (DMI) is actually a collection of three separate indicators combined into one. 
    /// Directional Movement consists of the Average Directional Index (ADX), Plus Directional Indicator (+DI) and Minus Directional Indicator (-DI). 
    /// DMI's purposes is to define whether or not there is a trend present. It does not take direction into account at all. 
    /// The other two indicators (+DI and -DI) are used to compliment the ADX. They serve the purpose of determining trend direction. 
    /// By combining all three, a technical analyst has a way of determining and measuring a trend's strength as well as its direction.
    /// https://www.tradingview.com/wiki/Directional_Movement_(DMI)
    /// </summary>
    public class DMI : TechnicalIndicatorBase,  ITechnicalIndicator
    {
        #region << Public Properties >>

        /// <summary>
        /// Get or set the previous period
        /// </summary>        
        public Period PreviousPeriod { get; set; }        

        /// <summary>
        /// The total of periods used for calculate
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Mantains an exponential moving average of the +DM / Average True Range
        /// </summary>
        public double PreviousEMAPlusDI { get; set; }

        /// <summary>
        /// Mantains an exponential moving average of the +DM / Average True Range
        /// </summary>
        public double PreviousEMAMinusDI { get; set; }

        /// <summary>
        /// get or set the previous adx ema
        /// </summary>
        public double PreviousEmaADX { get; set; }

        /// <summary>
        /// Average True range
        /// </summary>        
        public AverageTrueRange Atr { get; set; }       

        /// <summary>
        /// Get the EMA multiplier
        /// </summary>
        [NotMapped]
        private double Multiplier
        {
            get { return (2.0 / (Length + 1.0)); }
        }

        #endregion

        #region << Constructor >>    

        public DMI() : this(PeriodType.Day, 0, 14) { }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="length"></param>
        public DMI(PeriodType type = PeriodType.Day, long stockExchangeId = 0, int length = 14, AverageTrueRange atr = null) : base(type, stockExchangeId)
        {
            Length = length;
            Atr = atr ?? new AverageTrueRange(type, stockExchangeId, length);        
        }

        #endregion

        #region << IStockExchangeIndicator Members >>

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get { return "DMI"; } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<double> Update(Period period, bool normalize = true)
        {
            var adx = 0.0;
            var plusDI = 0.0;
            var minusDI = 0.0;

            if (PreviousPeriod != null)
            {
                var upMove = period.High - PreviousPeriod.High;
                var downMove = period.Low - PreviousPeriod.Low;

                var plusDM = upMove > downMove && upMove > 0.0 ? upMove : 0.0;
                var minusDM = downMove > upMove && downMove > 0.0 ? downMove : 0.0;

                var atr = Atr.Update(period).First();
                var plusDMOverATR = atr != 0.0 ? plusDM / atr : 0.0;
                var minusDMOverATR =atr != 0.0 ? minusDM / atr : 0.0;
              
                PreviousEMAPlusDI = IndicatorUtils.EMA(plusDMOverATR , PreviousEMAPlusDI, Multiplier);
                PreviousEMAMinusDI = IndicatorUtils.EMA(minusDMOverATR, PreviousEMAMinusDI,  Multiplier);

                plusDI = 100 * PreviousEMAPlusDI;
                minusDI = 100 * PreviousEMAMinusDI;

                var val = (plusDI + minusDI) != 0.0 ? Math.Abs((plusDI - minusDI) / (plusDI + minusDI)) : 0.0;
                PreviousEmaADX = IndicatorUtils.EMA(val, PreviousEmaADX, Multiplier);

                adx = 100 * PreviousEmaADX; 
            }

            PreviousPeriod = period;

            Value = new double[3] { adx, plusDI, minusDI };
            
            return normalize ? Value.Select(v => Normalizers.DMI.Normalize(v)) : Value;
        }

        /// <summary>
        /// Save the indicator
        /// </summary>
        /// <param name="ctx"></param>
        public override void Save(DeepQStockContext ctx)
        {
            var dbObj = ctx.DMIs.Find(Id);

            if (dbObj == null)
            {
                ctx.DMIs.Add(this);
            }
            else
            {
                ctx.Entry(dbObj).CurrentValues.SetValues(this);
                ctx.Entry(dbObj).State = EntityState.Modified;

                dbObj.PreviousPeriod = PreviousPeriod;

            }
        }

        #endregion
    }
}
