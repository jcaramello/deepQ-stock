using DeepQStock.Agents;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using DeepQStock.Stocks;
using DeepQStock.Utils;
using Hangfire;
using ServiceStack.Redis;
using Newtonsoft.Json;

namespace DeepQStock.Server.Hubs
{
    public class AgentHub : Hub
    {
        #region << Private Properties >>       

        /// <summary>
        /// Agent Storage
        /// </summary>
        public BaseStorage<DeepRLAgentParameters> AgentStorage { get; set; }

        /// <summary>
        /// QNetwork Storage
        /// </summary>
        public BaseStorage<QNetworkParameters> QNetworkStorage { get; set; }

        /// <summary>
        /// Stock Exchange Storage
        /// </summary>
        public BaseStorage<StockExchangeParameters> StockExchangeStorage { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        public IRedisClient Client { get; set; }

        /// <summary>
        /// Gets or sets the agent listeners.
        /// </summary>
        /// <value>
        /// The agent listeners.
        /// </value>
        public IDictionary<string, string> AgentListeners{ get; set; }

        private static string GroupNameTemplate = "ListenersForAgent-{0}";

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub(IRedisClient client, BaseStorage<DeepRLAgentParameters> agentStorage, BaseStorage<QNetworkParameters> qnetworkStorage, BaseStorage<StockExchangeParameters> stockExchangeStorage)
        {
            Client = client;
            AgentStorage = agentStorage;
            QNetworkStorage = qnetworkStorage;
            StockExchangeStorage = stockExchangeStorage;
            AgentListeners = new Dictionary<string, string>();
            SubscribeToOnDayComplete();                      
        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeepRLAgentParameters> GetAll()
        {
            return AgentStorage.GetAll();
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public DeepRLAgentParameters GetById(long id)
        {            
            return AgentStorage.GetById(id);
        }

        /// <summary>
        /// Subscribes the specified agent identifier.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        public void Subscribe(long id)
        {
            var groupName = string.Format(GroupNameTemplate, id);

            // if the client its already listen for an agent, we need to remove it. For now they can only listen one agent at time
            if (AgentListeners.ContainsKey(Context.ConnectionId))
            {
                var oldGroup = AgentListeners[Context.ConnectionId];
                AgentListeners.Remove(Context.ConnectionId);
                Groups.Remove(Context.ConnectionId, oldGroup);
            }

            AgentListeners.Add(Context.ConnectionId, groupName);
            Groups.Add(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Save an agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public long Save(DeepRLAgentParameters agent)
        {
            var qNetwork = agent.QNetworkParameters;
            QNetworkStorage.Save(qNetwork);
            agent.QNetworkParametersId = qNetwork.Id;
            agent.QNetworkParameters = null;

            AgentStorage.Save(agent);

            Clients.All.onCreatedAgent(agent);

            return agent.Id;
        }

        /// <summary>
        /// Start the simulation of the agent agentId
        /// </summary>
        /// <param name="id"></param>
        public void Start(int id)
        {
            BackgroundJob.Enqueue<StockExchange>(s => s.Start(id));
            Subscribe(id);
        }


        #region << Private Methods >> 

        private void SubscribeToOnDayComplete()
        {
            using (var sub = Client.CreateSubscription())
            {                
                sub.OnMessage += (channel, message) =>
                {
                    var args = JsonConvert.DeserializeObject<OnDayComplete>(message);
                    if (channel == RedisPubSubChannels.OnDayComplete)
                    {
                        var groupName = string.Format(GroupNameTemplate, args.AgentId);
                        Clients.Group(groupName).onDayComplete(args);
                    }
                };
            }
        }
                               

        #endregion
    }
}
