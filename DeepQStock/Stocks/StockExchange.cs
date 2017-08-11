using DeepQStock.Enums;
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
using Hangfire.Server;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity;

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
        public DeepRLAgent Agent { get; set; }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>        
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
        public State CurrentState { get; set; }

        /// <summary>
        /// Gets the annual profits.
        /// </summary>        
        public double AnnualProfits
        {
            get { return Profits / TotalOfYears; }
        }

        /// <summary>
        /// Gets the annual rent.
        /// </summary>        
        public double AnnualRent
        {
            get { return AnnualProfits / Parameters.InitialCapital; }
        }


        /// <summary>
        /// Gets the profits.
        /// </summary>
        /// <returns></returns>        
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
        public double NetCapital
        {
            get { return CurrentState.Today.CurrentCapital + (CurrentState.Today.ActualPosition * CurrentState.Today.Close); }
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
        public IList<TechnicalIndicatorBase> DailyIndicators { get; set; }

        /// <summary>
        /// List of stock exchange  weekly indicators used in each state
        /// </summary>        
        public IList<TechnicalIndicatorBase> WeeklyIndicators { get; set; }

        /// <summary>
        /// List of stock exchange monthly indicators used in each state
        /// </summary>        
        public IList<TechnicalIndicatorBase> MonthlyIndicators { get; set; }

        /// <summary>
        /// Gets or sets the reward calculator.
        /// </summary>        
        public RewardCalculator RewardCalculator { get; set; }

        /// <summary>
        /// Gets or sets the redis manager.
        /// </summary>
        private RedisManager RedisManager { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// We Need this constructor for hangfire
        /// </summary>
        public StockExchange(RedisManager manager)
        {
            RedisManager = manager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockExchange" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>       
        /// <param name="manager">The manager.</param>
        /// <param name="agent">The agent.</param>
        /// <param name="provider">The provider.</param>
        /// <exception cref="System.Exception">You must pass a csv file path for load the simulated data</exception>
        public StockExchange(StockExchangeParameters parameters, RedisManager manager, DeepRLAgent agent, IDataProvider provider)
        {
            RedisManager = manager;
            Agent = agent;
            DataProvider = provider;
            Parameters = parameters;
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
            try
            {
                Initialize(agentId);

                if (Agent == null)
                    return;

                Simulate(token);

                using (var DbContext = new DeepQStockContext())
                {
                    var agentParameters = DbContext.DeepRLAgentParameters.Single(a => a.Id == agentId.Value);
                    agentParameters.Status = AgentStatus.Completed;

                    var result = new SimulationResult()
                    {
                        AgentId = Agent.Parameters.Id,
                        AnnualProfits = AnnualProfits,
                        AnnualRent = AnnualRent,
                        NetCapital = NetCapital,
                        Profits = Profits,
                        Earnings = Earnings,
                        Symbol = Agent.Parameters.StockExchange.Symbol,
                        CreatedOn = DateTime.Now,
                        TransactionCost = TransactionCost
                    };

                    DbContext.SimulationResults.Add(result);
                    DbContext.SaveChanges();

                    RedisManager.Publish(RedisPubSubChannels.OnSimulationComplete, JsonConvert.SerializeObject(new OnSimulationComplete() { AgentId = agentParameters.Id }));

                }
            }
            catch (JobAbortedException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ClearDesicions();
            }
            finally
            {
                Shutdown();
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
                foreach (var state in episode)
                {
                    CurrentState = state;
                    action = Agent.Decide(state, reward);
                    reward = Execute(action, Agent.Parameters.InOutStrategy);
                    DaysSimulated++;

                    if (currentYear == null || currentYear != CurrentState.Today.Date.Year)
                    {
                        currentYear = CurrentState.Today.Date.Year;
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

                token.ThrowIfCancellationRequested();
                episode = NextEpisode();

            } while (episode.Count() > 0);
        }


        /// <summary>
        /// Initialize the agent and the stock exchange
        /// </summary>
        /// <param name="agentId"></param>
        protected void Initialize(long? agentId)
        {
            using (var DbContext = new DeepQStockContext())
            {                
                if (agentId.HasValue)
                {
                    var agentParameters = DbContext.DeepRLAgentParameters
                                                   .Include(a => a.QNetwork)
                                                   .Include(a => a.StockExchange)
                                                   .Single(a => a.Id == agentId.Value);

                    Parameters = agentParameters.StockExchange;

                    DailyIndicators = InitializeIndicators(DbContext, PeriodType.Day);
                    WeeklyIndicators = InitializeIndicators(DbContext, PeriodType.Week);
                    MonthlyIndicators = InitializeIndicators(DbContext, PeriodType.Month);

                    Agent = new DeepRLAgent(agentParameters);

                    RewardCalculator = RewardCalculator.Use(RewardCalculatorType.WinningsOverLoosings);
                    ((DeepRLAgent)Agent).OnTrainingEpochComplete += (e, args) => RedisManager.Publish(RedisPubSubChannels.OnTrainingEpochComplete, JsonConvert.SerializeObject(args));

                    var experiences = DbContext.Experiences.Where(e => e.AgentId == agentParameters.Id).ToList();
                    
                    DataProvider = new CsvDataProvider(Parameters.CsvDataFilePath, Parameters.EpisodeLength);

                    if (agentParameters.Status == AgentStatus.Paused)
                    {
                        CurrentState = DbContext.States
                                                .Include(s => s.InternalPeriods)
                                                .Single(s => s.StockExchangeId == Parameters.Id);                        

                        DataProvider.Seek(CurrentState.Today.Date.AddDays(1));
                    }

                    if (agentParameters.Status == AgentStatus.Completed)
                    {
                        ClearDesicions();
                    }

                    agentParameters.Status = AgentStatus.Running;
                    DbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public void Shutdown()
        {
            using (var DbContext = new DeepQStockContext())
            {
                var agent = DbContext.DeepRLAgentParameters.Single(a => a.Id == Agent.Parameters.Id);

                if (agent.Status == AgentStatus.Removed)
                {
                    DbContext.DeepRLAgentParameters.Remove(agent);
                }
                else if (agent.Status == AgentStatus.Paused)
                {
                    Agent.SaveQNetwork();

                    CurrentState.StockExchangeId = Parameters.Id;
                    DbContext.States.AddOrUpdate(CurrentState);

                    foreach (var p in CurrentState.InternalPeriods)
                    {
                        DbContext.Periods.AddOrUpdate(p);
                    }  

                    foreach (var indicator in DailyIndicators.Concat(WeeklyIndicators).Concat(MonthlyIndicators))
                    {
                        indicator.Save(DbContext);                        
                    }
                }

                DbContext.SaveChanges();
            }
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
            var actionPrice = CurrentState.Today.Close;
            var capital = CurrentState.Today.CurrentCapital;
            var position = CurrentState.Today.ActualPosition;

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
                        CurrentState.Today.ActualPosition += actionsToQuantity;
                        CurrentState.Today.CurrentCapital -= operationCost + TransactionCost;
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
                CurrentState.Today.ActualPosition -= actionsToSell;
                CurrentState.Today.CurrentCapital += (actionsToSell * actionPrice) - TransactionCost;

            }

            if (position > 0)
            {
                Earnings = position * (CurrentState.Today.Close - CurrentState.Today.Open);
            }

            var reward = RewardCalculator.Calculate(this);
            return reward;
        }

        /// <summary>
        /// Generate the next state 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<State> NextEpisode()
        {
            var upcomingDays = DataProvider.NextDays();
            var episode = new List<State>();

            foreach (var upcomingDay in upcomingDays)
            {
                if (CurrentState != null && CurrentState.Today != null)
                {
                    upcomingDay.ActualPosition = CurrentState.Today.ActualPosition;
                    upcomingDay.CurrentCapital = CurrentState.Today.CurrentCapital;
                }
                else
                {
                    upcomingDay.CurrentCapital = Parameters.InitialCapital;
                }

                episode.Add(GenerateState(upcomingDay));
            }

            return episode;
        }

        /// <summary>
        /// Generates the state.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        private State GenerateState(Period upcomingDay)
        {
            var state = CurrentState != null ? CurrentState.Clone() : new State();

            state.UpdateLayers(upcomingDay, DailyIndicators, WeeklyIndicators, MonthlyIndicators);

            return state;
        }       

        /// <summary>
        /// Triggers the on day completed event.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="reward">The reward.</param>
        private void DayCompleted(ActionType action, double reward)
        {
            var dayCompleted = new OnDayComplete()
            {
                Agent = Agent.Parameters,
                AgentId = Agent.Parameters.Id,
                DayNumber = DaysSimulated,
                Date = CurrentState.Today.Date,
                SelectedAction = action,
                Reward = reward,
                AccumulatedProfit = Profits,
                AnnualProfits = AnnualProfits,
                AnnualRent = AnnualRent,
                TotalOfYears = TotalOfYears,
                Period = CurrentState.Today
            };


            using (var DbContext = new DeepQStockContext())
            {
                DbContext.DeepRLAgentParameters.Attach(Agent.Parameters);
                DbContext.OnDaysCompletes.Add(dayCompleted);
                DbContext.SaveChanges();
            }

            RedisManager.Publish(RedisPubSubChannels.OnDayComplete, JsonConvert.SerializeObject(dayCompleted));
        }

        /// <summary>
        /// Clears the desicions.
        /// </summary>
        private void ClearDesicions()
        {
            using (var DbContext = new DeepQStockContext())
            {
                DbContext.OnDaysCompletes.RemoveRange(Agent.Parameters.Decisions);
                DbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Intialize all the stock indicators
        /// </summary>
        private IList<TechnicalIndicatorBase> InitializeIndicators(DeepQStockContext DbContext, PeriodType type)
        {
            var atr = DbContext.AverageTrueRanges.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type);
            if (atr == null)
            {
                atr = new AverageTrueRange(type, Parameters.Id);
                DbContext.AverageTrueRanges.Add(atr);
            }

            var ma8 = DbContext.SimpleMovingAverages.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type && a.Size == 8);
            if (ma8 == null)
            {
                ma8 = new SimpleMovingAverage(type, Parameters.Id, 8);
                DbContext.SimpleMovingAverages.Add(ma8);
            }

            var ema20 = DbContext.ExponentialMovingAverages.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type && a.Size == 20);
            if (ema20 == null)
            {
                ema20 = new ExponentialMovingAverage(type, Parameters.Id, 20);
                DbContext.ExponentialMovingAverages.Add(ema20);
            }

            var ema50 = DbContext.ExponentialMovingAverages.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type && a.Size == 50);
            if (ema50 == null)
            {
                ema50 = new ExponentialMovingAverage(type, Parameters.Id, 50);
                DbContext.ExponentialMovingAverages.Add(ema50);
            }

            var ema200 = DbContext.ExponentialMovingAverages.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type && a.Size == 200);
            if (ema200 == null)
            {
                ema200 = new ExponentialMovingAverage(type, Parameters.Id, 200);
                DbContext.ExponentialMovingAverages.Add(ema200);
            }

            var rsi = DbContext.RSIs.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type);
            if (rsi == null)
            {
                rsi = new RSI(type, Parameters.Id);
                DbContext.RSIs.Add(rsi);
            }

            var dmi = DbContext.DMIs.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type);
            if (dmi == null)
            {
                dmi = new DMI(type, Parameters.Id, atr: atr);
                DbContext.DMIs.Add(dmi);
            }

            var macd = DbContext.MACDs.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type);
            if (macd == null)
            {
                macd = new MACD(type, Parameters.Id);
                DbContext.MACDs.Add(macd);
            }

            var bollingerB = DbContext.BollingerBandsPercentBs.SingleOrDefault(a => a.StockExchangeId == Parameters.Id && a.Type == type);
            if (bollingerB == null)
            {
                bollingerB = new BollingerBandsPercentB(type, Parameters.Id);
                DbContext.BollingerBandsPercentBs.Add(bollingerB);
            }

            return new List<TechnicalIndicatorBase>() { ma8, ema20, ema50, ema200, atr, rsi, dmi, macd, bollingerB };
        }

        #endregion

    }
}
