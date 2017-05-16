﻿using DeepQStock.Config;
using DeepQStock.Enums;
using DeepQStock.Indicators;
using System;
using System.Collections.Generic;
using DeepQStock.Utils;

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
        /// Gets or sets the data provider.
        /// </summary>
        public IDataProvider DataProvider { get; set; }

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

            DataProvider = new CsvDataProvider(Parameters.CsvFilePath, Parameters.NumberOfPeriods, Parameters.StartDate);
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
                    if (CurrentState == null)
                    {                        
                        state.Today.CurrentCapital = Parameters.InitialCapital;                        
                    }

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
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private double Execute(ActionType action)
        {
            return 0.0;
        }

        /// <summary>
        /// Generate the next state 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<State> NextEpisode()
        {
            var upcomingDays = DataProvider.NextDays();

            foreach (var upcomingDay in upcomingDays)
            {
                yield return GenerateState(upcomingDay);
            }
        }

        /// <summary>
        /// Generates the state.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        private State GenerateState(Period upcomingDay)
        {            
            var state = CurrentState != null ? CurrentState.Clone() : new State();

            foreach (var indicator in DailyIndicators)
            {
                var values = indicator.Update(upcomingDay);
                upcomingDay.Indicators.Add(indicator.Name, values);
            }

            state.DayLayer.Enqueue(upcomingDay);

            UpdateLayer(state.WeekLayer, upcomingDay, WeeklyIndicators, upcomingDay.Date.IsStartOfWeek());
            UpdateLayer(state.MonthLayer, upcomingDay, MonthlyIndicators, upcomingDay.Date.IsStartOfMonth());

            return state;
        }

        /// <summary>
        /// Updates a layer of the state.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="upcomingDay">The upcoming day.</param>
        /// <param name="Indicators">The indicators.</param>
        private void UpdateLayer(CircularQueue<Period> layer, Period upcomingDay, IEnumerable<ITechnicalIndicator> Indicators, bool createNewPeriod)
        {
            Period current = null;
            if (!layer.IsEmpty && !createNewPeriod)
            {
                current = layer.Peek();
                current.Merge(upcomingDay);
            }
            else
            {
                current = upcomingDay.Clone();
            }

            foreach (var i in Indicators)
            {
                var values = i.Update(current);
                if (current.Indicators.ContainsKey(i.Name))
                {
                    current.Indicators[i.Name] = values;
                }
                else
                {
                    upcomingDay.Indicators.Add(i.Name, values);
                }
            }
        }


        #endregion

    }
}
