using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Stocks;
using StackExchange.Redis;
using System;

namespace DeepQStock.Storage
{
    public class RedisContext
    {
        //Storages
        public BaseStorage<StockExchangeParameters> StockExchange { get; set; }
        public BaseStorage<DeepRLAgentParameters> Agent { get; set; }
        public BaseStorage<QNetworkParameters> QNetwork { get; set; }
        public BaseStorage<SimulationResult> SimulationResult { get; set; }


        // Privates
        private ISubscriber Subscriber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisContext"/> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public RedisContext(IConnectionMultiplexer redis)
        {
            QNetwork = new BaseStorage<QNetworkParameters>(redis);
            Agent = new BaseStorage<DeepRLAgentParameters>(redis);
            StockExchange = new BaseStorage<StockExchangeParameters>(redis);
            SimulationResult = new BaseStorage<SimulationResult>(redis);

            Subscriber = redis.GetSubscriber();
        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Publish(string channel, string message)
        {
            Subscriber.Publish(channel, message);
        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler)
        {
            Subscriber.Subscribe(channel, handler);
        }
    }
}
