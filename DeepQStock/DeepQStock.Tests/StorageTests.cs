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

        /// <summary>
        /// Gets or sets the period storage.
        /// </summary>
        public PeriodStorage PeriodStorage { get; set; }

        /// <summary>
        /// Gets or sets the period storage.
        /// </summary>
        public StateStorage StateStorage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageTests"/> class.
        /// </summary>
        public StorageTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");            

            PeriodStorage = new PeriodStorage(redis);
            StateStorage = new StateStorage(redis);
        }

        #endregion

        #region << Tests >>

        /// <summary>
        /// Test_s the period storage_ crud.
        /// </summary>
        [TestMethod]
        public void Test_PeriodStorage_CRUD()
        {
            var period = DataGenerator.GetCompleteSamplePeriod();

            PeriodStorage.Save(period);

            period.Id.Should().BeGreaterThan(0);

            var retreivedPeriod = PeriodStorage.GetById(period.Id);

            retreivedPeriod.ShouldBeEquivalentTo(period);

            retreivedPeriod.Close = period.Close + 1;
            PeriodStorage.Save(retreivedPeriod);

            retreivedPeriod = PeriodStorage.GetById(period.Id);

            retreivedPeriod.Close.Should().Be(period.Close + 1);

            var all = PeriodStorage.GetAll();
            all.Count().Should().BeGreaterThan(0);

            PeriodStorage.Delete(retreivedPeriod);

            var deletedPeriod = PeriodStorage.GetById(retreivedPeriod.Id);
            deletedPeriod.Should().BeNull();
        }


        /// <summary>
        /// Test_s the period storage_ crud.
        /// </summary>
        [TestMethod]
        public void Test_StateStorage_CRUD()
        {
            var state = DataGenerator.GetCompleteSampleState();

            StateStorage.Save(state);

            state.Id.Should().BeGreaterThan(0);

            var retreivedState = StateStorage.GetById(state.Id);

            retreivedState.ShouldBeEquivalentTo(state);

            retreivedState.Size = state.Size + 1;
            StateStorage.Save(retreivedState);

            retreivedState = StateStorage.GetById(state.Id);

            retreivedState.Size.Should().Be(state.Size + 1);

            var all = StateStorage.GetAll();
            all.Count().Should().BeGreaterThan(0);

            StateStorage.Delete(retreivedState);

            var deleteState = StateStorage.GetById(retreivedState.Id);
            deleteState.Should().BeNull();
        }

        #endregion
    }
}
