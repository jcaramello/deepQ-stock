using DeepQStock.Domain;
using DeepQStock.Enums;

namespace DeepQStock.Agents
{   
    public interface IAgent
    {
        /// <summary>
        /// Base Parameters
        /// </summary>
        BaseAgentParameters Parameters { get; }

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
        void Save(); 
    }
}
