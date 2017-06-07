using DeepQStock.Domain;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class StateStorage : BaseStorage<State>
    {
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStorage"/> class.
        /// </summary>
        /// <param name="manager">The CTX.</param>
        public StateStorage(IRedisClientsManager manager) : base(manager)
        {
        }

        #endregion

        #region << IStorage Members >>       

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<State> GetAll()
        {
            IEnumerable<State> all = null;

            Execute((client, states) =>
            {
                all = states.GetAll().Select(s => s.Clone());                 
                foreach (var state in all)
                {
                    LoadLayers(client, state);
                }                
            });            

            return all;
        }

        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override State GetById(long id)
        {
            State state = null;
            Execute((client, states) => 
            {
                state = states.GetById(id);

                if (state != null)
                {
                    // We have to do this trick beacuase Redis is created the State object using reflection and it's not intializating the state in correct way
                    state = state.Clone();
                    LoadLayers(client, state);
                }                
            });            

            return state;
        }

        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <param name="item">The item.</param>        
        public override void Save(State item)
        {
            Execute((client, states) =>
            {
                var statePeriods = item.DayLayer.Concat(item.WeekLayer).Concat(item.MonthLayer);
                SaveLayers(client, statePeriods);

                if (item.Id == 0)
                {
                    item.Id = states.GetNextSequence();
                }

                item.PeriodIds = item.FlattenPeriods.Select(p => p.Id).ToList();
                states.Store(item);
            });
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Loads the layers.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="state">The state.</param>
        private void LoadLayers(IRedisClient client, State state)
        {            
            var periods = client.As<Period>().GetByIds(state.PeriodIds);
            foreach (var p in periods.OrderByDescending(p => p.Date))
            {
                switch (p.PeriodType)
                {
                    case Enums.PeriodType.Day:
                        state.DayLayer.Enqueue(p);
                        break;
                    case Enums.PeriodType.Week:
                        state.WeekLayer.Enqueue(p);
                        break;
                    case Enums.PeriodType.Month:
                        state.MonthLayer.Enqueue(p);
                        break;                    
                }
            }
        }

        /// <summary>
        /// Saves the layers.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="layer">The layer.</param>
        private void SaveLayers(IRedisClient client, IEnumerable<Period> layer)
        {
            var periods = client.As<Period>();
            var unsavedPeriods = layer.Where(p => p.Id == 0).ToList() ;

            foreach (var period in unsavedPeriods)
            {
                period.Id = periods.GetNextSequence();
            }

            if (unsavedPeriods.Count > 0)
            {
                periods.StoreAll(unsavedPeriods);
            }            
        }

        #endregion

    }
}
