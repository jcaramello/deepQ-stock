using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Stocks;
using StackExchange.Redis;
using System;
using System.Linq;

namespace DeepQStock.Storage
{
    public class RedisManager
    {       
        // Privates
        private ISubscriber Subscriber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisManager"/> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public RedisManager(IConnectionMultiplexer redis)
        {           
            Subscriber = redis.GetSubscriber();
        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Publish(string channel, string message)
        {
            try
            {
                Subscriber.Publish(channel, message);
            }
            catch (Exception)
            {

               
            }
        }

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler)
        {
            try
            {
                Subscriber.Subscribe(channel, handler);
            }
            catch (Exception)
            {

                
            }
        }       
    }
}
