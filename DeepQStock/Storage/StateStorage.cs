using DeepQStock.Domain;
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
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStorage"/> class.
        /// </summary>
        /// <param name="conn">The CTX.</param>
        public StateStorage(IConnectionMultiplexer conn) : base(conn)
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

          
                all = GetAll().Select(s => s.Clone());                 
                foreach (var state in all)
                {
                    LoadLayers(state);
                }                
                    

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
           
                state = GetById(id);

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
        
                var statePeriods = item.DayLayer.Concat(item.WeekLayer).Concat(item.MonthLayer);
                SaveLayers(statePeriods);

                if (item.Id == 0)
                {
                    item.Id = GetNextSequence();
                }

                item.PeriodIds = item.FlattenPeriods.Select(p => p.Id).ToList();
                Save(item);
            
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
            //var periods = GetByIds(state.PeriodIds);
            //foreach (var p in periods.OrderByDescending(p => p.Date))
            //{
            //    switch (p.PeriodType)
            //    {
            //        case Enums.PeriodType.Day:
            //            state.DayLayer.Enqueue(p);
            //            break;
            //        case Enums.PeriodType.Week:
            //            state.WeekLayer.Enqueue(p);
            //            break;
            //        case Enums.PeriodType.Month:
            //            state.MonthLayer.Enqueue(p);
            //            break;                    
            //    }
            //}
        }

        /// <summary>
        /// Saves the layers.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="layer">The layer.</param>
        private void SaveLayers(IEnumerable<Period> layer)
        {
          
            var unsavedPeriods = layer.Where(p => p.Id == 0).ToList() ;

            foreach (var period in unsavedPeriods)
            {
                period.Id = GetNextSequence();
            }

            if (unsavedPeriods.Count > 0)
            {
                //unsavedPeriods.ForEach(p => Save(p));
            }            
        }

        #endregion

    }
}
