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
        public RedisContext Context { get; set; }

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
        public AgentHub(RedisContext ctx)
        {
            Context = ctx;
            Context.Subscribe(RedisPubSubChannels.OnDayComplete, (c, a) => OnDayComplete(JsonConvert.DeserializeObject<OnDayComplete>(a)));                       
            AgentListeners = new Dictionary<string, IList<string>>();
        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeepRLAgentParameters> GetAll()
        {
            return Context.Agents.GetAll();
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public DeepRLAgentParameters GetById(long id)
        {
            var agent = Context.Agents.GetById(id);
            agent.Decisions = GetDecisions(id);

            return agent;
        }

        /// <summary>
        /// Gets the decisions.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IEnumerable<OnDayComplete> GetDecisions(long id)
        {
            return Context.OnDayCompleted.GetAll().Where(d => d.AgentId == id);
        }

        /// <summary>
        /// Called when [day complete].
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void OnDayComplete(OnDayComplete args)
        {
            var group = Clients.Group(string.Format(GroupNameTemplate, args.AgentId));
            group?.onDayComplete(args);
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

            var previousSubcription = AgentListeners.Where((KeyValuePair<string, IList<string>> i) => i.Value.Contains(base.Context.ConnectionId));
            if (previousSubcription.Count() > 0)
            {
                var previousGroupName = previousSubcription.First().Key;
                AgentListeners[previousGroupName].Remove(base.Context.ConnectionId);
                Groups.Remove(base.Context.ConnectionId, previousGroupName);
            }

            var listerners = AgentListeners[groupName];
            if (!listerners.Contains(base.Context.ConnectionId))
            {
                listerners.Add(base.Context.ConnectionId);
                Groups.Add(base.Context.ConnectionId, groupName);
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
            Context.QNetworks.Save(qNetwork);
            agent.QNetworkParametersId = qNetwork.Id;
            agent.QNetworkParameters = null;

            Context.Agents.Save(agent);

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
        public void Pause(long id)
        {
            var agent = Context.Agents.GetById(id);
            agent.Status = AgentStatus.Paused;

            Context.Agents.Save(agent);
            Stop(id);   
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
