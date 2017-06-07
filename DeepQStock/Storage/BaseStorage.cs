using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class BaseStorage<T> : IStorage<T> where T : BaseModel
    {

        #region << Protected Properties >>  

        /// <summary>
        /// Gets or sets the redis manager.
        /// </summary>
        private IRedisClientsManager RedisManager { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage"/> class.
        /// </summary>        
        public BaseStorage(IRedisClientsManager manager)
        {
            RedisManager = manager;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public TResult Execute<TResult>(Func<IRedisClient, IRedisTypedClient<T>, TResult> action)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                var periods = redis.As<T>();
                return action.Invoke(redis, periods);
            };
        }

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Execute(Action<IRedisClient, IRedisTypedClient<T>> action)
        {
            using (IRedisClient redis = RedisManager.GetClient())
            {
                var periods = redis.As<T>();
                action.Invoke(redis, periods);
            };
        }

        #endregion

        #region << IStorage Members >>

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return Execute((client, items) => items.GetAll());
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual T GetById(long id)
        {
            return Execute((client, items) => items.GetById(id));
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public virtual void Save(T model)
        {
            Execute((client, items) =>
            {
                if (model.Id == 0)
                {
                    model.Id = items.GetNextSequence();
                }

                items.Store(model);
            });

        }


        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="model">The item.</param>
        public virtual void Delete(T model)
        {
            Execute((client, items) => items.DeleteById(model.Id));
        }

        #endregion
    }
}
