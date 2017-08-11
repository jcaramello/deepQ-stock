using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Indicators;
using DeepQStock.Stocks;
using System;
using System.Data.Entity;
using System.IO;

namespace DeepQStock.Storage
{
    [DbConfigurationType(typeof(DeepQStockDbConfiguration))]
    public partial class DeepQStockContext : DbContext
    {
        #region << Public Properties >> 

        public DbSet<DeepRLAgentParameters> DeepRLAgentParameters { get; set; }
        public DbSet<QNetworkParameters> QNetworkParameters { get; set; }
        public DbSet<StockExchangeParameters> StockExchangeParameters{ get; set; }        
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Period> Periods{ get; set; }
        public DbSet<State> States{ get; set; }
        public DbSet<SimulationResult> SimulationResults{ get; set; }
        public DbSet<OnDayComplete> OnDaysCompletes { get; set; }

        public DbSet<AverageTrueRange> AverageTrueRanges { get; set; }
        public DbSet<BollingerBandsPercentB> BollingerBandsPercentBs { get; set; }
        public DbSet<DMI> DMIs{ get; set; }
        public DbSet<SimpleMovingAverage> SimpleMovingAverages { get; set; }
        public DbSet<ExponentialMovingAverage> ExponentialMovingAverages { get; set; }
        public DbSet<MACD> MACDs { get; set; }
        public DbSet<RSI> RSIs{ get; set; }

        #endregion

        #region << Private Properties >>

        

        #endregion

        #region << Constructor >> 
        
        public DeepQStockContext(): base("DeepQStockDB")
        {
        }      
       
        #endregion
    }
}
