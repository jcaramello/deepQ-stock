using deepQStock.Config;
using deepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace deepQStock
{
    public class Agent
    {
        #region << Private Properties >> 

        public AgentParameters Parameters { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="Agent"/> class.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public Agent(Action<AgentParameters> initializer = null)
        {
            Parameters = new AgentParameters();
            initializer?.Invoke(Parameters);
            
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Decides the next action
        /// </summary>
        /// <param name="st">The st.</param>
        /// <returns></returns>
        public ActionType Decide(State st, double reward_rt)
        {
            return ActionType.Buy;
        }

        /// <summary>
        /// Saves the experience.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="at">At.</param>
        /// <param name="rt_plus_1">The rt_plus_1.</param>
        /// <param name="st_plus_1">The st_plus_1.</param>
        public void SaveExperience(State st, ActionType at, double rt_plus_1, State st_plus_1)
        {

        }

        public IList<object> GenerateMiniBatch()
        {
            return null;
        }


        #endregion
    }
}
