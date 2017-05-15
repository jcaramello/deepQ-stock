using DeepQStock.Config;
using DeepQStock.Enums;
using DeepQStock.Indicators;
using LINQtoCSV;
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
        /// Gets the agent.
        /// </summary>        
        private IAgent Agent
        {
            get
            {
                return Parameters.Agent;
            }
        }

        /// <summary>
        /// Get or sets the periods
        /// </summary>
        private IEnumerable<Period> Periods { get; set; }

        /// <summary>
        /// Mantaing the numbers of periods already simulated by the agent
        /// </summary>
        public int EpisodeSimulated { get; set; }

        /// <summary>
        /// Current State
        /// </summary>
        public State CurrentState { get; set; }

        /// <summary>
        /// List of stock exchange  daily indicators used in each state
        /// </summary>
        public IList<ITechnicalIndicator> DailyIndicators
        {
            get
            {
                return Parameters.DailyIndicators;
            }
        }

        /// <summary>
        /// List of stock exchange  weekly indicators used in each state
        /// </summary>
        public IList<ITechnicalIndicator> WeeklyIndicators
        {
            get
            {
                return Parameters.WeeklyIndicators;
            }
        }

        /// <summary>
        /// List of stock exchange monthly indicators used in each state
        /// </summary>
        public IList<ITechnicalIndicator> MonthlyIndicators
        {
            get
            {
                return Parameters.MonthlyIndicators;
            }
        }


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

            Periods = GetAllPeriodFromCsv();
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
                    CurrentState = state;
                    action = Agent.Decide(state, reward);
                    reward = Execute(action);
                }

                //Agent.OnEpisodeComplete();
                EpisodeSimulated++;
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
            var state = new State();
            var nroOfPeriodSimulated = EpisodeSimulated * Parameters.NumberOfPeriods;
            state.Periods = Periods.Reverse().Skip(nroOfPeriodSimulated).Take(Parameters.NumberOfPeriods).ToList();

            if (CurrentState == null)
            {
                state.CurrentPeriod.CurrentCapital = Parameters.InitialCapital;
            }

            foreach (var p in state.Periods)
            {
                foreach (var i in DailyIndicators)
                {
                    var values = i.Calculate(p);
                    p.Indicators.Add(i.Name, values);
                }
            }

            return state;
        }

        /// <summary>
        /// Get all Period
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Period> GetAllPeriodFromCsv()
        {
            CsvFileDescription descriptor = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureInfo = System.Globalization.CultureInfo.InvariantCulture
            };

            CsvContext ctx = new CsvContext();

            return ctx.Read<Period>(Parameters.CsvFilePath, descriptor);
        }

        #endregion

    }
}
