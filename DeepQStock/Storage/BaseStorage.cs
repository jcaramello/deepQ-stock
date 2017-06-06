using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class BaseStorage<T> where T : class
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
    }
}
