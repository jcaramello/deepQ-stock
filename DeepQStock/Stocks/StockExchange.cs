﻿using DeepQStock.Enums;
using DeepQStock.Indicators;
using System;
using System.Collections.Generic;
using DeepQStock.Utils;
using DeepQStock.Domain;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;
using DeepQStock.Agents;
using DeepQStock.Storage;
using Hangfire;

namespace DeepQStock.Stocks
{
    /// <summary>
    /// Encapsulate the funcionality of a financial stock exchange, this class will be used for simulate the angent'enviroment.
    /// The main Goal it's generate the the states that the agent percibe, the different market indicator
    /// presents in each state, and finally it's responsable for calculate each reward given to the agent
    /// </summary>
    public class StockExchange
    {
        #region << Events >>

        //public event EventHandler<OnDayCompleteArgs> OnDayComplete;

        #endregion

        #region << Private Properties >>

        /// <summary>
        /// Stock exchange parameters
        /// </summary>
        public StockExchangeParameters Parameters { get; set; }

        /// <summary>
        /// Gets the agent.
        /// </summary>        
        [JsonIgnore]
        public IAgent Agent { get; set; }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        [JsonIgnore]
        public IDataProvider DataProvider { get; set; }

        /// <summary>
        /// Mantaing the numbers of periods already simulated by the agent
        /// </summary>
        public int EpisodeSimulated { get; set; }

        /// <summary>
        /// Gets or sets the days simulated.
        /// </summary>
        public int DaysSimulated { get; set; }

        /// <summary>
        /// Current State
        /// </summary>
        [JsonIgnore]
        public State CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the current period.
        /// </summary>
        [JsonIgnore]
        public Period CurrentPeriod
        {
            get
            {
                return CurrentState != null ? CurrentState.Today : null;
            }
        }

        /// <summary>
        /// Gets the annual profits.
        /// </summary>
        [JsonIgnore]
        public double AnnualProfits
        {
            get { return Profits / TotalOfYears; }
        }

        /// <summary>
        /// Gets the annual rent.
        /// </summary>
        [JsonIgnore]
        public double AnnualRent
        {
            get { return AnnualProfits / Parameters.InitialCapital; }
        }


        /// <summary>
        /// Gets the profits.
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public double Profits
        {
            get
            {
                return NetCapital - Parameters.InitialCapital;
            }
        }

        /// <summary>
        /// Net Capital
        /// </summary>
        [JsonIgnore]
        public double NetCapital
        {
            get { return CurrentPeriod.CurrentCapital + (CurrentPeriod.ActualPosition * CurrentPeriod.Close); }
        }

        /// <summary>
        /// Gets or sets the earning.
        /// </summary>
        public double Earnings { get; set; }

        /// <summary>
        /// Gets or sets the transaction cost.
        /// </summary>
        public double TransactionCost { get; set; }

        /// <summary>
        /// Gets or sets the total of years.
        /// </summary>
        public int TotalOfYears { get; set; }

        /// <summary>
        /// List of stock exchange  daily indicators used in each state
        /// </summary>
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
        public IList<ITechnicalIndicator> MonthlyIndicators
        {
            get
            {
                return Parameters.MonthlyIndicators;
            }
        }

        /// <summary>
        /// Gets or sets the reward calculator.
        /// </summary>
        /// <value>
        /// The reward calculator.
        /// </value>
        public Func<StockExchange, double> RewardCalculator { get; set; }

        /// <summary>
        /// Gets or sets the storage manager.
        /// </summary>      
        public RedisContext StorageManager { get; set; }


        #endregion

        #region << Constructor >>

        /// <summary>
        /// We Need this constructor for hangfire
        /// </summary>
        public StockExchange(RedisContext manager)
        {
            StorageManager = manager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockExchange" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="agent">The agent.</param>
        /// <param name="provider">The provider.</param>
        /// <exception cref="System.Exception">You must pass a csv file path for load the simulated data</exception>
        public StockExchange(StockExchangeParameters parameters, RedisContext manager, IAgent agent, IDataProvider provider)
        {
            StorageManager = manager;
            Agent = agent;
            DataProvider = provider;
            Parameters = parameters ?? new StockExchangeParameters();
            RewardCalculator = parameters.RewardCalculator;
        }

        #endregion

        #region << Public Methods >>     

        /// <summary>
        /// Initialize and execute the simulation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="agentId"></param>
        public void Run(IJobCancellationToken token, long? agentId = null)
        {
            Initialize(agentId);

            if (Agent == null)
                return;

            try
            {
                Simulate(token);
            }
            catch (Exception)
            {
                Shutdown();
                throw;
            }
        }         

        /// <summary>
        /// Simulate the enviroment and the agent integration
        /// </summary>
        protected void Simulate(IJobCancellationToken token)
        {            
            IEnumerable<State> episode = null;
            var action = ActionType.Wait;
            double reward = 0.0;
            int? currentYear = null;
            episode = NextEpisode();

            do
            {
                token.ThrowIfCancellationRequested();

                foreach (var state in episode)
                {
                    CurrentState = state;
                    action = Agent.Decide(state, reward);
                    reward = Execute(action, Agent.Parameters.InOutStrategy);
                    DaysSimulated++;

                    if (currentYear == null || currentYear != CurrentPeriod.Date.Year)
                    {
                        currentYear = CurrentPeriod.Date.Year;
                        TotalOfYears++;
                    }

                    DayCompleted(action, reward);

                    if (Parameters.SimulationVelocity > 0)
                    {
                        Thread.Sleep(Parameters.SimulationVelocity);
                    }
                }

                Agent.OnEpisodeComplete();
                EpisodeSimulated++;
                episode = NextEpisode();

            } while (episode != null);

            SaveResults();
        }


        /// <summary>
        /// Initialize the agent and the stock exchange
        /// </summary>
        /// <param name="agentId"></param>
        protected void Initialize(long? agentId)
        {
            if (agentId.HasValue)
            {
                var agentParameters = StorageManager.Agent.GetById(agentId.Value);
                agentParameters.QNetworkParameters = StorageManager.QNetwork.GetById(agentParameters.QNetworkParametersId);

                Agent = new DeepRLAgent(agentParameters);
                RewardCalculator = Stocks.RewardCalculators.WinningsOverLoosings;
                Parameters = StorageManager.StockExchange.GetById(agentParameters.StockExchangeParametersId);
                DataProvider = new CsvDataProvider(Parameters.CsvDataFilePath, Parameters.EpisodeLength);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
           
        }

        #endregion

        #region << Private Methods >>



        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private double Execute(ActionType action, double inOutStrategy)
        {
            var actionPrice = CurrentPeriod.Close;
            var capital = CurrentPeriod.CurrentCapital;
            var position = CurrentPeriod.ActualPosition;

            if (action == ActionType.Buy && capital > 0)
            {
                var totalToBuy = inOutStrategy * capital;
                if (totalToBuy > actionPrice)
                {
                    var actionsToQuantity = (int)Math.Truncate(totalToBuy / actionPrice);
                    TransactionCost = actionsToQuantity * actionPrice * Parameters.TransactionCost;
                    if (TransactionCost <= capital)
                    {
                        var operationCost = actionPrice * actionsToQuantity;
                        CurrentPeriod.ActualPosition += actionsToQuantity;
                        CurrentPeriod.CurrentCapital -= operationCost + TransactionCost;
                    }
                }
            }
            else if (action == ActionType.Sell && position > 0)
            {
                var actionsToSell = (int)Math.Truncate(inOutStrategy * position);
                if (actionsToSell == 0)
                {
                    actionsToSell = position;
                }

                TransactionCost = actionsToSell * actionPrice * Parameters.TransactionCost;
                CurrentPeriod.ActualPosition -= actionsToSell;
                CurrentPeriod.CurrentCapital += (actionsToSell * actionPrice) - TransactionCost;

            }

            if (position > 0)
            {
                Earnings = position * (CurrentPeriod.Close - CurrentPeriod.Open);
            }

            var reward = RewardCalculator(this);
            return reward;
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
                if (CurrentPeriod != null)
                {
                    upcomingDay.ActualPosition = CurrentPeriod.ActualPosition;
                    upcomingDay.CurrentCapital = CurrentPeriod.CurrentCapital;
                }
                else
                {
                    upcomingDay.CurrentCapital = Parameters.InitialCapital;
                }

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

            UpdateLayer(PeriodType.Week, state.WeekLayer, upcomingDay, WeeklyIndicators);
            UpdateLayer(PeriodType.Month, state.MonthLayer, upcomingDay, MonthlyIndicators);

            return state;
        }

        /// <summary>
        /// Updates a layer of the state.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="upcomingDay">The upcoming day.</param>
        /// <param name="Indicators">The indicators.</param>
        private void UpdateLayer(PeriodType type, CircularQueue<Period> layer, Period upcomingDay, IEnumerable<ITechnicalIndicator> indicators)
        {
            Period current = null;
            bool needNewPeriod = type == PeriodType.Week ? upcomingDay.Date.IsStartOfWeek() : upcomingDay.Date.IsStartOfMonth();

            if (layer.IsEmpty || needNewPeriod)
            {
                current = upcomingDay.Clone();
                current.PeriodType = type;
                layer.Enqueue(current);
            }
            else
            {
                current = layer.Peek();
                current.Merge(upcomingDay);
            }

            foreach (var indicator in indicators)
            {
                var newValues = indicator.Update(current);
                if (current.Indicators.ContainsKey(indicator.Name))
                {
                    current.Indicators[indicator.Name] = newValues;
                }
                else
                {
                    current.Indicators.Add(indicator.Name, newValues);
                }
            }
        }


        /// <summary>
        /// Triggers the on day completed event.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="reward">The reward.</param>
        private void DayCompleted(ActionType action, double reward)
        {
            var args = new OnDayComplete()
            {
                AgentId = Agent.Parameters.Id,
                DayNumber = DaysSimulated,
                Date = CurrentPeriod.Date,
                SelectedAction = action,
                Reward = reward,
                AccumulatedProfit = Profits,
                AnnualProfits = AnnualProfits,
                AnnualRent = AnnualRent,
                TotalOfYears = TotalOfYears,
                Period = CurrentPeriod
            };

            StorageManager.Publish(RedisPubSubChannels.OnDayComplete, JsonConvert.SerializeObject(args));
        }

        /// <summary>
        /// Save simulation result for staticts
        /// </summary>
        private void SaveResults()
        {
            var result = new SimulationResult()
            {
                agentId = Agent.Parameters.Id,
                AnnualProfits = AnnualProfits,
                AnnualRent = AnnualRent,
                NetCapital = NetCapital,
                Profits= Profits,
                Symbol = Agent.Parameters.Symbol,
                TransactionCost = TransactionCost
            };

            StorageManager.SimulationResult.Save(result);            

        }

        #endregion

    }
}
