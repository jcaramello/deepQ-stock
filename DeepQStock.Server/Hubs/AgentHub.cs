using DeepQStock.Agents;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using DeepQStock.Stocks;
using DeepQStock.Utils;
using Hangfire;
using Newtonsoft.Json;
using System.Linq;
using DeepQStock.Enums;
using System.Collections.Concurrent;

namespace DeepQStock.Server.Hubs
{
    public class AgentHub : Hub
    {
        public static ConcurrentDictionary<long, string> ActiveAgents = new ConcurrentDictionary<long, string>();

        #region << Private Properties >>       

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        public StorageManager Manager { get; set; }

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
        public AgentHub(StorageManager manager)
        {
            Manager = manager;
            Manager.Subscribe(RedisPubSubChannels.OnDayComplete, a => OnDayComplete(JsonConvert.DeserializeObject<OnDayComplete>(a)));                       
            AgentListeners = new Dictionary<string, IList<string>>();
        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeepRLAgentParameters> GetAll()
        {
            return Manager.AgentStorage.GetAll();
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public DeepRLAgentParameters GetById(long id)
        {
            return Manager.AgentStorage.GetById(id);
        }

        /// <summary>
        /// Called when [day complete].
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void OnDayComplete(OnDayComplete args)
        {
            Clients.Group(string.Format(GroupNameTemplate, args.AgentId))?.onDayComplete(args);
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
            Manager.QNetworkStorage.Save(qNetwork);
            agent.QNetworkParametersId = qNetwork.Id;
            agent.QNetworkParameters = null;

            Manager.AgentStorage.Save(agent);

            Clients.All.onCreatedAgent(agent);

            return agent.Id;
        }

        /// <summary>
        /// Start the simulation of the agent agentId
        /// </summary>
        /// <param name="id"></param>
        public string Start(long id)
        {
            var jobId = BackgroundJob.Enqueue<StockExchange>(s => s.Run(JobCancellationToken.Null, id));
            
            ActiveAgents.TryAdd(id, jobId);
            Subscribe(id);

            return jobId;
        }

        /// <summary>
        /// Start the simulation of the agent agentId
        /// </summary>
        /// <param name="id"></param>
        public void Stop(long id)
        {
            string jobId = null;
            ActiveAgents.TryRemove(id, out jobId);

            if (!string.IsNullOrEmpty(jobId))
            {
                BackgroundJob.Delete(jobId);                
            }            
        }

        /// <summary>
        /// Start the simulation of the agent agentId
        /// </summary>
        /// <param name="id"></param>
        public void Reset(int id)
        {
           
        }

    }
}
