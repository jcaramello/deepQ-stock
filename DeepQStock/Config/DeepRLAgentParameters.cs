using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Config
{
    public class DeepRLAgentParameters
    {
        #region << Public Properties >>

        /// <summary>
        /// Gets or sets the exploration frequency.
        /// </summary>        
        public double eGreedyProbability { get; set; }

        /// <summary>
        /// Gets or sets the in and out strategy that the agent will be use for buy and sell actions.
        /// It should be a value between 0 and 1 that represent the percentage that the agent buy o sell in each transaction.
        /// </summary>
        public double InOutStrategy { get; set; }

        /// <summary>
        /// Gets or sets the size of the training mini batch.
        /// </summary>
        public int MiniBatchSize { get; set; }

        /// <summary>
        /// Gets or sets the discount factor.
        /// </summary>
        public double DiscountFactor { get; set; }

        /// <summary>
        /// Gets or sets the hidden layers count.
        /// </summary>
        public int HiddenLayersCount { get; set; }

        /// <summary>
        /// Gets or sets the neuron count for hidden layers.
        /// </summary>
        public int NeuronCountForHiddenLayers { get; set; }

        /// <summary>
        /// Get or set the size of the internal agent's memory replay
        /// </summary>
        public int MemoryReplaySize { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepRLAgentParameters"/> class.
        /// </summary>
        public DeepRLAgentParameters()
        {
            eGreedyProbability = 0.001;
            InOutStrategy = 0.25;
            MiniBatchSize = 10;
            DiscountFactor = 0.5;
            HiddenLayersCount = 2;
            NeuronCountForHiddenLayers = HiddenLayersCount * 2;
            MemoryReplaySize = 2000;
        }

        #endregion
    }
}
