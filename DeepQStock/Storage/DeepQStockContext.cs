using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Indicators;
using DeepQStock.Stocks;
using System;
using System.Data.Entity;
using System.Linq;
using System.IO;

namespace DeepQStock.Storage
{
    [DbConfigurationType(typeof(DeepQStockDbConfiguration))]
    public partial class DeepQStockContext : DbContext
    {
        #region << Public Properties >> 

        public DbSet<DeepRLAgentParameters> DeepRLAgentParameters { get; set; }
        public DbSet<QNetworkParameters> QNetworkParameters { get; set; }
        public DbSet<StockExchangeParameters> StockExchangeParameters { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<IndicatorValue> IndicatorValues { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<SimulationResult> SimulationResults { get; set; }
        public DbSet<OnDayComplete> OnDaysCompletes { get; set; }

        public DbSet<AverageTrueRange> AverageTrueRanges { get; set; }
        public DbSet<BollingerBandsPercentB> BollingerBandsPercentBs { get; set; }
        public DbSet<DMI> DMIs { get; set; }
        public DbSet<SimpleMovingAverage> SimpleMovingAverages { get; set; }
        public DbSet<ExponentialMovingAverage> ExponentialMovingAverages { get; set; }
        public DbSet<MACD> MACDs { get; set; }
        public DbSet<RSI> RSIs { get; set; }

        #endregion

        #region << Private Properties >>



        #endregion

        #region << Constructor >> 

        public DeepQStockContext() : base("DeepQStockDB")
        {
        }

        #endregion       

        #region << Public Methods >>

        /// <summary>
        /// Check fi an entity is attached or not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsAttached<T>(T entity) where T : BaseModel
        {
            return Set<T>().Local.Any(e => e == entity);
        }

        /// <summary>
        /// Remove an agent with all the relations
        /// </summary>
        /// <param name="agent"></param>
        public void RemoveAgent(DeepRLAgentParameters agent)
        {
            ClearAgent(agent);
            Experiences.RemoveRange(Experiences.Where(e => e.AgentId == agent.Id));
            SimulationResults.RemoveRange(SimulationResults.Where(e => e.AgentId == agent.Id));
            DeepRLAgentParameters.Remove(agent);
        }

        /// <summary>
        /// Clear Agent
        /// </summary>
        /// <param name="agent"></param>
        public void ClearAgent(DeepRLAgentParameters agent)
        {
            Entry(agent).Reference(a => a.StockExchange).Load();
            var stockId = agent.StockExchange.Id;           

            AverageTrueRanges.RemoveRange(AverageTrueRanges.Where(a => a.StockExchangeId == stockId));
            BollingerBandsPercentBs.RemoveRange(BollingerBandsPercentBs.Where(a => a.StockExchangeId == stockId));
            DMIs.RemoveRange(DMIs.Where(a => a.StockExchangeId == stockId));
            ExponentialMovingAverages.RemoveRange(ExponentialMovingAverages.Where(a => a.StockExchangeId == stockId));
            MACDs.RemoveRange(MACDs.Where(a => a.StockExchangeId == stockId));
            RSIs.RemoveRange(RSIs.Where(a => a.StockExchangeId == stockId));
            SimpleMovingAverages.RemoveRange(SimpleMovingAverages.Where(a => a.StockExchangeId == stockId));

            var periodsToDelete = Periods.Where(p => p.StockExchangeId == stockId);
            IndicatorValues.RemoveRange(IndicatorValues.Where(i => i.Period.StockExchangeId == stockId));
            Periods.RemoveRange(periodsToDelete);

            States.RemoveRange(States.Where(e => e.StockExchangeId == stockId));
            OnDaysCompletes.RemoveRange(OnDaysCompletes.Where(e => e.Agent.Id == agent.Id));
        }

        #endregion
    }
}
