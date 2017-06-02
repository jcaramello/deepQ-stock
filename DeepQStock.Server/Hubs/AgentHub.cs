using DeepQStock.Server.Models;
using Microsoft.AspNet.SignalR;
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

        public IList<AgentModel> Agents { get; set; }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgentHub()
        {
            Agents = new List<AgentModel>()
            {
                new AgentModel() { Id = 1, Name ="Agent APPL",  Symbol = "MSFT" },
                new AgentModel() { Id = 2, Name ="Agent MSFT", Symbol = "APPL"},
            };

        }

        #endregion

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AgentModel> GetAll()
        {
            return Agents;
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public AgentModel GetById(long id)
        {
            return Agents.SingleOrDefault(a => a.Id == id); 
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public void CreateAgent(string name)
        {
            var agent = new AgentModel()
            {
                Id = Agents.Max(a => a.Id) + 1,
                Name = name
            };

            Agents.Add(agent);

            Clients.All.onCreatedAgent(agent);
        }
    }
}
