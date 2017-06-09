using DeepQStock.Agents;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using DeepQStock.Stocks;
using DeepQStock.Utils;
using Hangfire;

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

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub(BaseStorage<DeepRLAgentParameters> agentStorage, BaseStorage<QNetworkParameters> qnetworkStorage, BaseStorage<StockExchangeParameters> stockExchangeStorage)
        {
            AgentStorage = agentStorage;
            QNetworkStorage = qnetworkStorage;
            StockExchangeStorage = stockExchangeStorage;
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
        /// <param name="agentId"></param>
        public void Start(int agentId)
        {
            BackgroundJob.Enqueue<StockExchange>(s => s.Start(agentId));
        }
    }
}
