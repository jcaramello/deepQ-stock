using DeepQStock.Server.Models;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Hubs
{
    public class StockExchangeHub : Hub
    {

        #region << Public Properties >>

        /// <summary>
        /// Agent Storage
        /// </summary>
        public BaseStorage<Agent> AgentStorage { get; set; }

        /// <summary>
        /// QNetwork Storage
        /// </summary>
        public BaseStorage<QNetwork> QNetworkStorage { get; set; }

        /// <summary>
        /// Agent Storage
        /// </summary>
        public BaseStorage<StockExchange> StockExchangeStorage { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StockExchangeHub(BaseStorage<Agent> agentStorage, BaseStorage<QNetwork> qnetworkStorage, BaseStorage<StockExchange> stockExchangeStorage)
        {
            AgentStorage = agentStorage;
            QNetworkStorage = qnetworkStorage;
            StockExchangeStorage = stockExchangeStorage;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Save a stock
        /// </summary>
        /// <param name="stock"></param>
        public void Save(StockExchange stock)
        {
            var agent = stock.Agent;
            stock.Agent = null;           

            var qNetwork = agent.QNetwork;
            QNetworkStorage.Save(qNetwork);
            agent.QNetworkId = qNetwork.Id;
            agent.QNetwork = null;

            AgentStorage.Save(agent);
            stock.AgentId = agent.Id;
            stock.Agent = null;

            StockExchangeStorage.Save(stock);

            Clients.All.createdAgent(agent);
        }

        #endregion
    }
}
