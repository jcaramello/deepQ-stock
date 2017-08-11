using DeepQStock.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using DeepQStock.Utils;
using Encog.Engine.Network.Activation;
using DeepQStock.Domain;
using System.IO;
using DeepQStock.Storage;

namespace DeepQStock.Agents
{
    /// <summary>
    /// Implement an angent combining reinforcement learning and deep learning
    /// The agent use the Q-Learning algorithim with experience replay 
    /// Also use a neural network for implement a Q state-action value function aproximator
    /// </summary>
    public class DeepRLAgent 
    {
        #region << Private Properties >>        

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

        /// <summary>
        /// Internal memory relplay
        /// </summary>
        private CircularQueue<Experience> MemoryReplay { get; set; }

        /// <summary>
        /// Occurs when [on training complete].
        /// </summary>
        public event EventHandler<OnTrainingCompleteArgs> OnTrainingComplete;

        /// <summary>
        /// Occurs when [on training epoch complete].
        /// </summary>
        public event EventHandler<OnTrainingEpochCompleteArgs> OnTrainingEpochComplete;

        /// <summary>
        /// Gets the network path.
        /// </summary>
        private string TempFolder
        {
            get
            {
                return string.Format(@"{0}deepQStock", Path.GetTempPath());
            }
        }

        /// <summary>
        /// Gets the network path.
        /// </summary>
        private string NetworkPath
        {
            get
            {
                return string.Format(@"{0}\Agent-{1}", TempFolder, Parameters.Id);
            }
        }

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Base params
        /// </summary>
        public DeepRLAgentParameters Parameters { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepRLAgent"/> class.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public DeepRLAgent(DeepRLAgentParameters parameters = null)
        {
            Parameters = parameters ?? new DeepRLAgentParameters();
            RandomGenerator = new Random();
            MemoryReplay = new CircularQueue<Experience>(Parameters.MemoryReplaySize);
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
                InitializeQNetwork(state);
            }

            var previuosState = CurrentState;
            CurrentState = state;

            if (previuosState != null)
            {
                AddExperience(previuosState, CurrentAction, reward, CurrentState);
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

        /// <summary>
        /// Saves the specified path.
        /// </summary>        
        public void SaveQNetwork()
        {
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            Q.Save(NetworkPath);           
        }

        /// <summary>
        /// Set pass experiences to the agent
        /// </summary>
        /// <param name="experiences"></param>
        public void SetExperiences(IEnumerable<Experience> experiences)
        {
            foreach (var e in experiences)
            {
                MemoryReplay.Enqueue(e);
            }
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
            var validActions = GetActions();
            var maxAction = Q[CurrentState].Where(i => validActions.Contains(i.Key)).MaxBy(i => i.Value).Key;

            if (probability <= Parameters.eGreedyProbability)
            {
                var randomActions = validActions.Where(a => a != maxAction).ToList();
                var randomIndex = RandomGenerator.Next(randomActions.Count);
                CurrentAction = randomActions[randomIndex];
            }
            else
            {
                CurrentAction = maxAction;
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
        private void AddExperience(State from, ActionType action, double reward, State to)
        {
            var experience = new Experience()
            {
                AgentId = Parameters.Id,
                From = from,
                Action = CurrentAction,
                Reward = reward,
                To = to
            };

            MemoryReplay.Enqueue(experience);
        }

        /// <summary>
        /// Generates the a mini batch of randoms sample for train and update Q-network wegihts.
        /// </summary>
        /// <returns></returns>
        private IList<Experience> GenerateMiniBatch()
        {
            var experiences = new List<Experience>();

            if (MemoryReplay.Count <= Parameters.MiniBatchSize)
            {
                experiences = MemoryReplay.ToList();
            }
            else
            {
                var indexes = Enumerable.Range(0, MemoryReplay.Count - 1).OrderBy(x => RandomGenerator.Next());
                experiences = MemoryReplay.Where((e, i) => indexes.Contains(i)).ToList();
            }

            using (var ctx = new DeepQStockContext())
            {
                foreach (var experience in experiences)
                {
                    if (experience.Id > 0 && !ctx.Set<Experience>().Local.Any(e => e.Id == experience.Id))
                    {
                        ctx.Experiences.Attach(experience);
                    }                    

                    if (experience.From == null)
                    {
                        ctx.Entry(experience).Reference(e => e.From).Load();
                    }

                    if (experience.To == null)
                    {
                        ctx.Entry(experience).Reference(e => e.To).Load();
                    }
                }
            }

            return experiences.ToList();            
        }

        /// <summary>
        /// Gets the list of valid actions in the current state.
        /// </summary>
        /// <returns></returns>
        private IList<ActionType> GetActions()
        {
            var actions = new List<ActionType>() { ActionType.Wait };
            var avaliableCapital = CurrentState.Today.CurrentCapital * Parameters.InOutStrategy;

            if (avaliableCapital >= CurrentState.Today.Close)
            {
                actions.Add(ActionType.Buy);
            }

            if (CurrentState.Today.ActualPosition > 0)
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
            var actions = Enum.GetValues(typeof(ActionType)).Cast<ActionType>();

            foreach (var experience in mini_batch)
            {
                var targetValues = new Dictionary<ActionType, double>();
                var estimatedValues = Q[experience.From];

                foreach (var action in actions)
                {
                    targetValues[action] = QUpdate(experience, action, estimatedValues[action]);
                }

                trainingData.Add(new Tuple<State, double[]>(experience.From, targetValues.Values.ToArray()));
            }

            Q.Train(trainingData);
            OnTrainingComplete?.Invoke(this, new OnTrainingCompleteArgs());
        }

        /// <summary>
        /// Calculates the q update.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="action">The action.</param>
        /// <param name="estimated">The estimated.</param>
        /// <returns></returns>
        private double QUpdate(Experience e, ActionType action, double estimated)
        {
            var estimation = e.Action == action ? e.Reward + (Parameters.DiscountFactor * estimated) : estimated;

            return Normalizers.Reward.Normalize(estimation);
        }

        /// <summary>
        /// Initializes the internal neural network.
        /// </summary>
        /// <param name="state">The state.</param>
        private void InitializeQNetwork(State state)
        {
            if (File.Exists(NetworkPath))
            {
                Q = new QNetwork(NetworkPath);
            }
            else
            {
                var inputLength = state.ToArray().Length;

                Q = new QNetwork(p =>
                {
                    p.TrainingError = Parameters.QNetwork.TrainingError;
                    p.MaxIterationPerTrainging = Parameters.QNetwork.MaxIterationPerTrainging;

                    // Input Layer
                    p.Layers.Add(new LayerParameters(null, false, inputLength));

                    // Hidden Layers
                    for (int i = 0; i < Parameters.QNetwork.HiddenLayersCount; i++)
                    {
                        p.Layers.Add(new LayerParameters(new ActivationTANH(), true, Parameters.QNetwork.NeuronCountForHiddenLayers));
                    }

                    // Output Layer
                    p.Layers.Add(new LayerParameters(new ActivationTANH(), false, 3));
                });

                Q.OnTrainingEpochComplete += (e, a) =>
                {
                    a.AgentId = this.Parameters.Id;
                    OnTrainingEpochComplete?.Invoke(e, a);
                };
            }
        }


        #endregion
    }

}
