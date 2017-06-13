using DeepQStock.Agents;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using DeepQStock.Stocks;
using DeepQStock.Utils;
using Hangfire;
using ServiceStack.Redis;
using Newtonsoft.Json;
using System.Linq;
using DeepQStock.Enums;

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
        public IRedisClientsManager Manager { get; set; }

        /// <summary>
        /// Gets or sets the agent listeners.
        /// </summary>        
        public IDictionary<string, IList<string>> AgentListeners { get; set; }

        /// <summary>
        /// The group name template
        /// </summary>
        private static string GroupNameTemplate = "ListenersForAgent-{0}";



        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub(IRedisClientsManager manager, BaseStorage<DeepRLAgentParameters> agentStorage, BaseStorage<QNetworkParameters> qnetworkStorage, BaseStorage<StockExchangeParameters> stockExchangeStorage)
        {
            Manager = manager;

            IRedisPubSubServer server = Manager.CreatePubSubServer(RedisPubSubChannels.OnDayComplete, (c, a) => OnDayComplete(JsonConvert.DeserializeObject<OnDayComplete>(a)));
            server.Start();

            AgentStorage = agentStorage;
            QNetworkStorage = qnetworkStorage;
            StockExchangeStorage = stockExchangeStorage;
            AgentListeners = new Dictionary<string, IList<string>>();
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
        /// Called when [day complete].
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void OnDayComplete(OnDayComplete args)
        {
            Clients.Group(string.Format(GroupNameTemplate, args.AgentId)).onDayComplete(args);
        }

        /// <summary>
        /// Subscribes the specified agent identifier.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        public void Subscribe(long id)
        {
            var groupName = string.Format(GroupNameTemplate, id);

            if (!AgentListeners.ContainsKey(groupName))
            {
                AgentListeners[groupName] = new List<string>();
            }

            var previousSubcription = AgentListeners.Where(i => i.Value.Contains(Context.ConnectionId));
            if (previousSubcription.Count() > 0)
            {
                var previousGroupName = previousSubcription.First().Key;
                AgentListeners[previousGroupName].Remove(Context.ConnectionId);
                Groups.Remove(Context.ConnectionId, previousGroupName);
            }

            var listerners = AgentListeners[groupName];
            if (!listerners.Contains(Context.ConnectionId))
            {
                listerners.Add(Context.ConnectionId);
                Groups.Add(Context.ConnectionId, groupName);
            }

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
        public string Start(int id)
        {
            var jobId = BackgroundJob.Enqueue<StockExchange>(s => s.Start(id));
            StockExchange.Jobs.TryAdd(jobId, StockExchangeStatus.Running);

            Subscribe(id);

            return jobId;
        }

        public void Pause(string jobId)
        {
            StockExchange.Jobs[jobId] = StockExchangeStatus.Paused;
        }

        public void Continue(string jobId)
        {
            StockExchange.Jobs[jobId] = StockExchangeStatus.Running;
        }
    }
}
