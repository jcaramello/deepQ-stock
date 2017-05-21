using DeepQStock.Domain;
using DeepQStock.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepQStock
{
    public interface IAgent
    {
        /// <summary>
        /// Decides the next action to execute
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="reward">The reward.</param>
        /// <returns></returns>
        ActionType Decide(State state, double reward);

        /// <summary>
        /// Called when [episode complete].
        /// </summary>        
        void OnEpisodeComplete();  
    }
}
