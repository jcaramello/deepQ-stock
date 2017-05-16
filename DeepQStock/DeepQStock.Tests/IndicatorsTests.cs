using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeepQStock.Indicators;
using System.Linq;

namespace DeepQStock.Tests
{
    [TestClass]
    public class IndicatorsTests : BaseTest
    {       
        #region  << Tests >>

        [TestMethod]
        public void MovingAverage_Test()
        {
            var sma = new SimpleMovingAverage(10);
            var ema = new ExponentialMovingAverage(10);
            var periods = DataGenerator.GetSamplePeriods();
            var sma_value = 0.0;
            var ema_value = 0.0;

            foreach (var p in periods)
            {
                sma_value = sma.Update(p).First();                
                ema_value = ema.Update(p).First();
            }

            Assert.AreEqual(sma_value, 22.209);
            Assert.AreEqual(Math.Round(ema_value, 2), 22.22);
        }

        #endregion
    }
}
