using DeepQStock.Config;
using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeepQStock.Utils;
using Encog.Engine.Network.Activation;

namespace DeepQStock
{
    /// <summary>
    /// Implement an angent combining reinforcement learning and deep learning
    /// The agent use the Q-Learning algorithim with experience replay 
    /// Also use a neural network for implement a Q state-action value function aproximator
    /// </summary>
    public class DeepRLAgent : IAgent
    {
        #region << Private Properties >> 

        /// <summary>
        /// Gets or sets the agent's parameters.
        /// </summary>
        private DeepRLAgentParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the current state st
        /// </summary>
        private State CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the selected action.
        /// </summary>
        private ActionType CurrentAction { get; set; }

        /// <summary>
        /// Gets or sets the probability generator.
        /// </summary>
        private Random RandomGenerator { get; set; }

        /// <summary>
        /// State-action function Q
        /// </summary>
        /// </value>
        private QNetwork Q { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepRLAgent"/> class.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public DeepRLAgent(Action<DeepRLAgentParameters> initializer = null)
        {
            Parameters = new DeepRLAgentParameters();
            initializer?.Invoke(Parameters);
            RandomGenerator = new Random();
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Decides the next action
        /// </summary>
        /// <param name="state">The st.</param>
        /// <returns></returns>
        public ActionType Decide(State state, double reward)
        {
            if (Q == null)
            {
                InitializeNetwork(state);
            }

            var previuosState = CurrentState;
            CurrentState = state;

            if (previuosState != null)
            {
                var experience = new Experience()
                {
                    From = previuosState,
                    Action = CurrentAction,
                    Reward = reward,
                    To = CurrentState
                };

                SaveExperience(experience);
            }

            return PolicyPi();
        }

        /// <summary>
        /// Called when [episode complete].
        /// </summary>
        /// <param name="episode">The episode.</param>
        public void OnEpisodeComplete()
        {
            UpdateKnowledge();
        }

        #endregion


        #region << Private methods >>

        /// <summary>
        /// Select an a random action a' with probability e 
        /// otherwise select the action a' that max a' Q(s, a')
        /// </summary>
        /// <returns></returns>
        private ActionType PolicyPi()
        {
            var probability = RandomGenerator.NextDouble();

            if (probability <= Parameters.ExplorationFrequency)
            {
                CurrentAction = (ActionType)RandomGenerator.Next(4);
            }
            else
            {
                var validActions = GetActions();
                CurrentAction = Q[CurrentState].Where(i => validActions.Contains(i.Key)).MaxBy(i => i.Value).Key;
            }

            return CurrentAction;
        }

        /// <summary>
        /// Saves the a experience in memory replay.
        /// </summary>
        /// <param name="state">The st.</param>
        /// <param name="action">At.</param>
        /// <param name="reward">The rt_plus_1.</param>
        /// <param name="nextState">The st_plus_1.</param>
        private void SaveExperience(Experience experience)
        {

        }

        /// <summary>
        /// Generates the a mini batch of randoms sample for train and update Q-network wegihts.
        /// </summary>
        /// <returns></returns>
        private IList<Experience> GenerateMiniBatch()
        {
            var minibatch = new List<Experience>();

            for (int i = 0; i < Parameters.MiniBatchSize; i++)
            {
                minibatch.Add(new Experience());
            }

            return minibatch;
        }

        /// <summary>
        /// Gets the list of valid actions in the current state.
        /// </summary>
        /// <returns></returns>
        private IList<ActionType> GetActions()
        {
            var actions = new List<ActionType>() { ActionType.Wait };
            var avaliableCapital = CurrentState.CurrentPeriod.CurrentCapital * Parameters.InOutStrategy;

            if (avaliableCapital >= CurrentState.CurrentPeriod.Close)
            {
                actions.Add(ActionType.Buy);
            }

            if (CurrentState.CurrentPeriod.ActualPosicion > 0)
            {
                actions.Add(ActionType.Sell);
            }

            return actions;
        }

        /// <summary>
        /// Implement the Q Learning update rule using experience replay
        /// </summary>
        private void UpdateKnowledge()
        {
            var mini_batch = GenerateMiniBatch();
            var trainingData = new List<Tuple<State, double[]>>();

            foreach (var experience in mini_batch)
            {
                var targetValues = new double[3];
                var estimatedValues = Q[experience.From];

                targetValues[0] = experience.Reward + (Parameters.DiscountFactor * estimatedValues[ActionType.Buy]);
                targetValues[1] = experience.Reward + (Parameters.DiscountFactor * estimatedValues[ActionType.Sell]);
                targetValues[2] = experience.Reward + (Parameters.DiscountFactor * estimatedValues[ActionType.Wait]);

                trainingData.Add(new Tuple<State, double[]>(experience.From, targetValues));
            }

            Q.Train(trainingData);
        }

        /// <summary>
        /// Initializes the internal neural network.
        /// </summary>
        /// <param name="state">The state.</param>
        private void InitializeNetwork(State state)
        {
            Q = new QNetwork(p =>
            {
                // Input Layer
                p.Layers.Add(new LayerParameters(null, true, state.ToArray().Length));

                // Hidden Layers
                for (int i = 0; i < Parameters.HiddenLayersCount; i++)
                {
                    p.Layers.Add(new LayerParameters(new ActivationSigmoid(), true, Parameters.NeuronCountForHiddenLayers));
                }

                // Output Layer
                p.Layers.Add(new LayerParameters(new ActivationSigmoid(), false, 3));
            });
        }


        #endregion
    }

}
