using DeepQStock.Domain;
using DeepQStock.Storage;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Linq;

namespace DeepQStock.Tests
{
    [TestClass]
    public class StorageTests : BaseTest
    {
        #region << Setup >>         

        public DeepQStockContext Database { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageTests"/> class.
        /// </summary>
        public StorageTests()
        {
            Database = new DeepQStockContext();

        }

        #endregion

        #region << Tests >>

        /// <summary>
        /// Test_s the period storage_ crud.
        /// </summary>
        [TestMethod]
        public void Test_PeriodStorage_CRUD()
        {
            //var period = DataGenerator.GetCompleteSamplePeriod();

            //Database.Periods.Save(period);

            //period.Id.Should().BeGreaterThan(0);

            //var retreivedPeriod = Database.Periods.GetById(period.Id);

            //retreivedPeriod.ShouldBeEquivalentTo(period);

            //retreivedPeriod.Close = period.Close + 1;
            //Database.Periods.Save(retreivedPeriod);

            //retreivedPeriod = Database.Periods.GetById(period.Id);

            //retreivedPeriod.Close.Should().Be(period.Close + 1);

            //var all = Database.Periods.GetAll();
            //all.Count().Should().BeGreaterThan(0);

            //Database.Periods.Delete(retreivedPeriod);

            //var deletedPeriod = Database.Periods.GetById(retreivedPeriod.Id);
            //deletedPeriod.Should().BeNull();
        }


        /// <summary>
        /// Test_s the period storage_ crud.
        /// </summary>
        [TestMethod]
        public void Test_StateStorage_CRUD()
        {
            //var state = DataGenerator.GetCompleteSampleState();

            //Database.States.Save(state);

            //state.Id.Should().BeGreaterThan(0);

            //var retreivedState = Database.States.GetById(state.Id);

            //retreivedState.ShouldBeEquivalentTo(state);
            //retreivedState.Today.Date.Should().Be(state.Today.Date);

            //retreivedState.Size = state.Size + 1;
            //Database.States.Save(retreivedState);

            //retreivedState = Database.States.GetById(state.Id);

            //retreivedState.Size.Should().Be(state.Size + 1);

            //var all = Database.States.GetAll();
            //all.Count().Should().BeGreaterThan(0);

            //Database.States.Delete(retreivedState);

            //var deleteState = Database.States.GetById(retreivedState.Id);
            //deleteState.Should().BeNull();
        }

        #endregion
    }
}
