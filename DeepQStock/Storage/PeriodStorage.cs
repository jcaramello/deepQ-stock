using DeepQStock.Domain;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class PeriodStorage : BaseStorage<Period>, IStorage<Period>
    {
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage"/> class.
        /// </summary>        
        public PeriodStorage(IRedisClientsManager manager) : base(manager)
        {
        }

        #endregion

        #region << IStorage Members >>        

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Period> GetAll()
        {
            return Execute((client, periods) => periods.GetAll());
        }

        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Period GetById(long id)
        {
            return Execute((client, periods) => periods.GetById(id));
        }

        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Save(Period item)
        {
            Execute((client, periods) =>
            {
                if (item.Id == 0)
                {
                    item.Id = periods.GetNextSequence();
                }

                periods.Store(item);
            });
        }

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(Period item)
        {
            Execute((client, periods) => periods.DeleteById(item.Id));
        }

        #endregion

    }
}
