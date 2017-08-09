using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Indicators;
using DeepQStock.Stocks;
using System.Data.Entity;
using System.IO;

namespace DeepQStock.Storage
{
    [DbConfigurationType(typeof(DeepQStockDbConfiguration))]
    public partial class DeepQStockContext : DbContext
    {
        #region << Public Properties >> 

        public DbSet<DeepRLAgentParameters> Agents { get; set; }
        public DbSet<QNetworkParameters> QNetworks { get; set; }
        public DbSet<StockExchangeParameters> Stocks{ get; set; }        
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Period> Periods{ get; set; }
        public DbSet<State> States{ get; set; }
        public DbSet<SimulationResult> SimulationResults{ get; set; }
        public DbSet<OnDayComplete> OnDaysCompleted { get; set; }

        public DbSet<AverageTrueRange> AverageTrueRange { get; set; }
        public DbSet<BollingerBandsPercentB> BollingerBandsPercentB { get; set; }
        public DbSet<DMI> DMI{ get; set; }
        public DbSet<SimpleMovingAverage> SimpleMovingAverage { get; set; }
        public DbSet<ExponentialMovingAverage> ExponentialMovingAverage { get; set; }
        public DbSet<MACD> MACD { get; set; }
        public DbSet<RSI> RSI{ get; set; }

        #endregion

        #region << Private Properties >>

        

        #endregion

        #region << Constructor >> 
        
        public DeepQStockContext(): base()
        {

        }


       
        #endregion
    }
}
