using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Collections;
using System.Collections.Generic;

namespace DeepQStock.Storage
{
    /// <summary>
    /// Implement a persisten layer using a Redis database
    /// </summary>    
    public class RedisContext
    {
        #region << Private Properties >>               

        /// <summary>
        /// Gets or sets the connection multiplexer.
        /// </summary>
        private IConnectionMultiplexer ConnMultiplexer { get; set; }

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        private ISerializer Serializer { get; set; }

        /// <summary>
        /// lock object
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Template for redis keyes
        /// </summary>
        private const string KeyTemplate = "{0}:id";

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Cache Client
        /// </summary>
        public readonly ICacheClient Client;

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisContext" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        public RedisContext(string server = "localhost", int port = 6379)
        {
            Serializer = new NewtonsoftSerializer();
            ConnMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { { server, port } },
                AllowAdmin = true
            });

            Client = new StackExchangeRedisCacheClient(ConnMultiplexer, Serializer);        
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GenerateKey<T>()
        {            
            var key = string.Format(KeyTemplate, typeof(T).Name);
            long id = 0;

            lock (_lock)
            {                
                id = Client.Database.StringIncrement(key);                          
            }

            return string.Format("{0}:{1}", key, id);
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<string> GetKeys<T>()
        {
            var key = string.Format(KeyTemplate, typeof(T).Name);
            return Client.SearchKeys(key);
        }

        #endregion

    }
}
