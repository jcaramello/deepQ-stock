using DeepQStock.Domain;
using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Tests
{
    public static class DataGenerator
    {
        /// <summary>
        /// Gets the sample periods.
        /// </summary>
        /// <returns></returns>
        public static IList<Period> GetSamplePeriodsWithCloseValue()
        {
            return new List<Period>()
            {
                new Period() { Close = 22.27 },
                new Period() { Close = 22.19 },
                new Period() { Close = 22.08 },
                new Period() { Close = 22.17 },
                new Period() { Close = 22.18 },
                new Period() { Close = 22.13 },
                new Period() { Close = 22.23 },
                new Period() { Close = 22.43 },
                new Period() { Close = 22.24 },
                new Period() { Close = 22.29 },
                new Period() { Close = 22.15 }
            };
        }

        /// <summary>
        /// Gets the complete sample period.
        /// </summary>
        /// <returns></returns>
        public static Period GetCompleteSamplePeriod(PeriodType type = PeriodType.Day)
        {
            return new Period()
            {
                Date = DateTime.Today,
                ActualPosition = 100,
                Close = 10,
                CurrentCapital = 10000,
                High = 12,
                Low = 9,
                Open = 9.5,
                PeriodType = type,
                Volume = 1000000.0,
                //InternalIndicators = new List<IndicatorValue>()
                //{
                //    new IndicatorValue("Indicator1", new double[] { 1, 2}) ,
                //    new IndicatorValue( "Indicator2", new double[] { 0.5 } ),
                //    new IndicatorValue( "Indicator3", new double[] { 1, 2, 3}) ,
                //}
            };
        }

        /// <summary>
        /// Gets the complete sample state.
        /// </summary>
        /// <returns></returns>
        public static State GetCompleteSampleState()
        {
            var state = new State(1);

            //state.DayLayer.Enqueue(GetCompleteSamplePeriod(PeriodType.Day));
            //state.WeekLayer.Enqueue(GetCompleteSamplePeriod(PeriodType.Week));
            //state.MonthLayer.Enqueue(GetCompleteSamplePeriod(PeriodType.Month));

            return state;
        }
    }
}
