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
        public BaseStorage<AgentModel> AgentStorage { get; set; }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub(IRedisClientsManager manager)
        {
            AgentStorage = new BaseStorage<AgentModel>(manager);
        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AgentModel> GetAll()
        {
            return AgentStorage.Execute((c, a) => a.GetAll());
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public AgentModel GetById(long id)
        {
            return  AgentStorage.Execute((c, a) => a.GetById(id));
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public void CreateAgent(AgentModel agent)
        {            
         
            AgentStorage.Execute((c, a) =>
            {
                if (agent.Id == 0)
                {
                    agent.Id = a.GetNextSequence();
                }

                a.Store(agent);
            });
            
            Clients.All.onCreatedAgent(agent);
        }
    }
}
