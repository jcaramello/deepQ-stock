using DeepQStock.Enums;
using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Agents
{
    public class DeepRLAgentParameters : BaseAgentParameters
    {
        #region << Public Properties >>       
   
        /// <summary>
        /// Gets or sets the exploration frequency.
        /// </summary>        
        public double eGreedyProbability { get; set; }
        
        /// <summary>
        /// Gets or sets the size of the training mini batch.
        /// </summary>
        public int MiniBatchSize { get; set; }

        /// <summary>
        /// Gets or sets the discount factor.
        /// </summary>
        public double DiscountFactor { get; set; }

        /// <summary>
        /// Get or set the size of the internal agent's memory replay
        /// </summary>
        public int MemoryReplaySize { get; set; }

        /// <summary>
        /// Gets or sets the hidden layers count.
        /// </summary>
        public QNetworkParameters QNetworkParameters { get; set; }     
     
        /// <summary>
        /// QNetwork parameters id
        /// </summary>
        public long QNetworkParametersId { get; set; }

        /// <summary>
        /// Stock Exchange parameters id
        /// </summary>
        public long StockExchangeParametersId { get; set; }

        /// <summary>
        /// Agent Status
        /// </summary>
        public AgentStatus Status { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepRLAgentParameters"/> class.
        /// </summary>
        public DeepRLAgentParameters()
        {
            eGreedyProbability = 0.05;
            InOutStrategy = 0.33;
            MiniBatchSize = 50;
            DiscountFactor = 0.8;
            MemoryReplaySize = 500;
            QNetworkParameters = new QNetworkParameters();       
        }

        #endregion
    }
}
