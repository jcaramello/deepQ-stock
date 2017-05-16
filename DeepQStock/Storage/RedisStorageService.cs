using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    /// <summary>
    /// Implement a persisten layer using a Redis database
    /// </summary>    
    public class RedisStorageService : IStorageService
    {
        #region << Private Properties >>

        /// <summary>
        /// Redis Database
        /// </summary>
        protected readonly IDatabase Database;

        /// <summary>
        /// Cache Client
        /// </summary>
        protected readonly ICacheClient CacheClient;

        /// <summary>
        /// Gets or sets the connection multiplexer.
        /// </summary>
        public IConnectionMultiplexer ConnMultiplexer { get; set; }

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        public ISerializer Serializer { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisStorageService" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        public RedisStorageService(string server = "localhost", int port = 6379)
        {
            Serializer = new NewtonsoftSerializer();
            ConnMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {                
                //EndPoints = { { server, port} },
                AllowAdmin = true
            });

            CacheClient = new StackExchangeRedisCacheClient(ConnMultiplexer, Serializer);
            Database = CacheClient.Database;
        }    

        #endregion

        #region << IStorage Service >>

        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>        
        public T GetBy<T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>        
        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>        
        public void Save<T>(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>        
        public void Delete<T>(T item)
        {
            throw new NotImplementedException();
        }           

        #endregion
    }
}
