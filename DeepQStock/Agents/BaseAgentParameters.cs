using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Agents
{
    public class BaseAgentParameters : BaseModel
    {
        /// <summary>
        /// Companie Symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the in and out strategy that the agent will be use for buy and sell actions.
        /// It should be a value between 0 and 1 that represent the percentage that the agent buy o sell in each transaction.
        /// </summary>
        public double InOutStrategy { get; set; }


    }
}
