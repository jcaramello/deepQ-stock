using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

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
            // Make thread safe
            var keyGenerator = string.Format("{0}:id", typeof(T).Name);
            if (!Client.Exists(keyGenerator))
            {
                Client.Add<long>(keyGenerator, 0);
            }

            // Increment the key
            //Client.Database.

            var newKey = string.Format("{0}:{1}", keyGenerator, Client.Get<long>(keyGenerator));
            return newKey;    
        }

        #endregion

    }
}
