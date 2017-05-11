using deepQStock.Config;
using deepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace deepQStock
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
        public Agent Agent { get; set; }

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

            IList<Period> episode = null;
            State st = null;
            State st_minus_1 = null;
            double rt = 0.0;
            double rt_minus_1 = 0.0;
            ActionType at = ActionType.Wait;
            ActionType at_minus_1 = ActionType.Wait;

            while ((episode = NextEpisode()) != null)
            {
                foreach (var period in episode)
                {
                    st_minus_1 = st;
                    rt_minus_1 = rt;
                    at_minus_1 = at;
                                                        
                    st = GenerateState(st_minus_1, period);                    
                    at = Agent.Decide(st, rt_minus_1);
                    rt = Execute(at);

                    if (st_minus_1 != null)
                    {
                        Agent.SaveExperience(st_minus_1, at_minus_1, rt_minus_1, st);
                    }

                    var mini_batch = Agent.GenerateMiniBatch();
                    //train;

                }                
            }
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Generate the next state from 
        /// </summary>
        /// <returns></returns>
        private IList<Period> NextEpisode()
        {   
            // use the params episode length for read period from csv file
            return new List<Period>()
            {
                new Period()
            };
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
        private State GenerateState(State st_minus_1, Period period)
        {
            return null;
        }

        #endregion

    }
}
