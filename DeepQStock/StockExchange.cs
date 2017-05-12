using DeepQStock.Config;
using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock
{
    /// <summary>
    /// Encapsulate the funcionality of a financial stock exchange, this class will be used for simulate the angent'enviroment.
    /// The main Goal it's generate the the states that the agent percibe, the different market indicator
    /// presents in each state, and finally it's responsable for calculate each reward given to the agent
    /// </summary>
    public class StockExchange
    {

        #region << Private Properties >>

        /// <summary>
        /// Stock exchange parameters
        /// </summary>
        private StockExchangeParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>        
        public IAgent Agent { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StockExchange"/> class.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <exception cref="System.Exception">You must pass a csv file path for load the simulated data</exception>
        public StockExchange(Action<StockExchangeParameters> initializer = null)
        {
            Parameters = new StockExchangeParameters();
            initializer?.Invoke(Parameters);

            if (Parameters.CsvFilePath == null)
            {
                throw new Exception("You must pass a csv file path for load the simulated data");
            }
        }

        #endregion

        #region << Public Methods >>       

        /// <summary>
        /// Start the simulation
        /// </summary>
        public void Start()
        {
            if (Agent == null)
                return;

            IEnumerable<State> episode = null;            
            double reward = 0.0;            
            ActionType action = ActionType.Wait;            

            while ((episode = NextEpisode()) != null)
            {
                foreach (var state in episode)
                {                                                                                                
                    action = Agent.Decide(state, reward);
                    reward = Execute(action);                                                           
                }

                Agent.OnEpisodeComplete();
            }
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Generate the next state from 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<State> NextEpisode()
        {            
            for (int i = 0; i < Parameters.EpisodeLength; i++)
            {
                // read period from csv file and update the market indicators
                yield return GenerateState();
            }                        
        }

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private double Execute(ActionType action)
        {
            return 0.0;
        }

        /// <summary>
        /// Generates the state.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        private State GenerateState()
        {
            return null;
        }

        #endregion

    }
}
