using DeepQStock.Domain;
using DeepQStock.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class StateStorage : BaseStorage<State>
    {
        #region << Private >>         

        /// <summary>
        /// Gets or sets the period storage.
        /// </summary>
        private PeriodStorage PeriodStorage { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStorage"/> class.
        /// </summary>
        /// <param name="conn">The CTX.</param>
        public StateStorage(IConnectionMultiplexer conn) : base(conn)
        {
            PeriodStorage = new PeriodStorage(conn);
        }

        #endregion

        #region << IStorage Members >>       

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<State> GetAll()
        {
            return base.GetAll().Select(s => 
            {
                s.Clone();
                LoadLayers(s);

                return s;
            });                      
        }

        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override State GetById(long id)
        {
            State state = base.GetById(id);

            if (state != null)
            {
                // We have to do this trick beacuase Redis is created the State object using reflection and it's not intializating the state in correct way
                state = state.Clone();
                LoadLayers(state);
            }
            
            return state;
        }

        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <param name="item">The item.</param>        
        public override void Save(State item)
        {
            var periods = item.DayLayer.Concat(item.WeekLayer).Concat(item.MonthLayer).ToList();
            periods.ForEach(p => PeriodStorage.Save(p));

            if (item.Id == 0)
            {
                item.Id = GetNextSequence();
            }

            item.PeriodIds = periods.Select(p => p.Id).ToList();
            base.Save(item);
        }

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Delete(State item)
        {
            PeriodStorage.DeleteByIds(item.PeriodIds);
            base.Delete(item);
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Loads the layers.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="state">The state.</param>
        private void LoadLayers(State state)
        {
            var periods = PeriodStorage.GetByIds(state.PeriodIds);

            foreach (var p in periods.OrderBy(p => p.Date))
            {
                switch (p.PeriodType)
                {
                    case PeriodType.Day:
                        state.DayLayer.Enqueue(p);
                        break;
                    case PeriodType.Week:
                        state.WeekLayer.Enqueue(p);
                        break;
                    case PeriodType.Month:
                        state.MonthLayer.Enqueue(p);
                        break;
                }
            }
        }
        #endregion

    }
}
