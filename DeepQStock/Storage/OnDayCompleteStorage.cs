using DeepQStock.Stocks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class OnDayCompleteStorage : BaseStorage<OnDayComplete>
    {
        #region << Private >>         

        /// <summary>
        /// Gets or sets the period storage.
        /// </summary>
        private PeriodStorage PeriodStorage { get; set; }

        #endregion


        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="TechnicalIndicatorStorage"/> class.
        /// </summary>
        /// <param name="conn">The connection.</param>
        public OnDayCompleteStorage(IConnectionMultiplexer conn) : base(conn)
        {
            PeriodStorage = new PeriodStorage(conn);
        }

        #endregion

        #region << IStorage Members >>      

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override OnDayComplete GetById(long id)
        {
            var onDayComplete = base.GetById(id);
            onDayComplete.Period = PeriodStorage.GetById(onDayComplete.PeriodId);

            return onDayComplete;
        }

        /// <summary>
        /// Get all OnDayComplete items
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<OnDayComplete> GetAll()
        {
            var days = base.GetAll().ToList();
            var periods = PeriodStorage.GetByIds(days.Select(d => d.PeriodId).Distinct());
            foreach (var day in days)
            {
                day.Period = periods.Single(p => p.Id == day.PeriodId);
            }

            return days;
        }

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public override void Save(OnDayComplete onDayComplete)
        {
            PeriodStorage.Save(onDayComplete.Period);
            onDayComplete.PeriodId = onDayComplete.Period.Id;
            var period = onDayComplete.Period;
            onDayComplete.Period = null;
            base.Save(onDayComplete);
            onDayComplete.Period = period;
        }

        #endregion
    }
}