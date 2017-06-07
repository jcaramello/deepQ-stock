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
    public class AgentHub : Hub
    {

        #region << Private Properties >>       

        /// <summary>
        /// Agent Storage
        /// </summary>
        public BaseStorage<Agent> AgentStorage { get; set; }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub(BaseStorage<Agent> agentStorage)
        {
            AgentStorage = agentStorage;
        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Agent> GetAll()
        {
            return AgentStorage.GetAll();          
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public Agent GetById(long id)
        {
            return AgentStorage.GetById(id);
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public void Save(Agent agent)
        {        
            AgentStorage.Save(agent);            
            Clients.All.onCreatedAgent(agent);
        }
    }
}
