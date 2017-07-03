using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Indicators;
using DeepQStock.Stocks;
using StackExchange.Redis;
using System;

namespace DeepQStock.Storage
{
    public class RedisContext
    {
        //Storages
        public BaseStorage<StockExchangeParameters> StockExchanges { get; set; }
        public BaseStorage<DeepRLAgentParameters> Agents { get; set; }
        public BaseStorage<QNetworkParameters> QNetworks { get; set; }
        public BaseStorage<SimulationResult> SimulationResults { get; set; }
        public BaseStorage<TechnicalIndicatorBase> Indicators { get; set; }

        public StateStorage StateStorage { get; set; }

        // Privates
        private ISubscriber Subscriber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisContext"/> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public RedisContext(IConnectionMultiplexer redis)
        {
            QNetworks = new BaseStorage<QNetworkParameters>(redis);
            Agents = new BaseStorage<DeepRLAgentParameters>(redis);
            StockExchanges = new BaseStorage<StockExchangeParameters>(redis);
            SimulationResults = new BaseStorage<SimulationResult>(redis);
            Indicators = new BaseStorage<TechnicalIndicatorBase>(redis);

            StateStorage = new StateStorage(redis);

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
