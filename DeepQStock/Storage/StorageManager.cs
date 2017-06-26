using DeepQStock.Agents;
using DeepQStock.Stocks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class StorageManager
    {

        public BaseStorage<StockExchangeParameters> StockExchangeStorage { get; set; }
        public BaseStorage<DeepRLAgentParameters> AgentStorage { get; set; }
        public BaseStorage<QNetworkParameters> QNetworkStorage { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public StorageManager(IConnectionMultiplexer redis)
        {
            QNetworkStorage = new BaseStorage<QNetworkParameters>(redis);
            AgentStorage = new BaseStorage<DeepRLAgentParameters>(redis);
            StockExchangeStorage = new BaseStorage<StockExchangeParameters>(redis);
        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Publish(string channel, string message)
        {

        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Subscribe(string channel, Action<string> listener)
        {

        }
    }
}
