using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Storage;
using System.Collections.Generic;

namespace DeepQStock.Agents
{   
    public interface IAgent
    {
        /// <summary>
        /// Base Parameters
        /// </summary>
        DeepRLAgentParameters Parameters { get; }

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

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save(DeepQStockContext ctx);

        /// <summary>
        /// Set pass experiences to the agent
        /// </summary>
        /// <param name="experiences"></param>
        void SetExperiences(IEnumerable<Experience> experiences);
    }
}
