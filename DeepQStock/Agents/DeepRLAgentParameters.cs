﻿using DeepQStock.Enums;
using DeepQStock.Stocks;
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
        /// Get or sets the learning rate
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Get or set the size of the internal agent's memory replay
        /// </summary>
        public int MemoryReplaySize { get; set; }

        /// <summary>
        /// Gets or sets the hidden layers count.
        /// </summary>               
        public QNetworkParameters QNetwork { get; set; }   
        
        /// <summary>
        /// Agent Status
        /// </summary>
        public AgentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets my property.
        /// </summary>        
        public ICollection<OnDayComplete> Decisions { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepRLAgentParameters"/> class.
        /// </summary>
        public DeepRLAgentParameters()
        {
            eGreedyProbability = 0.1;
            InOutStrategy = 0.33;
            MiniBatchSize = 50;
            DiscountFactor = 0.8;
            LearningRate = 0.3;
            MemoryReplaySize = 500;
            QNetwork = new QNetworkParameters();
            Decisions = new List<OnDayComplete>();
        }

        #endregion
    }
}
