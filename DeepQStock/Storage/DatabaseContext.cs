using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Indicators;
using DeepQStock.Stocks;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class DatabaseContext
    {
        #region << Public Properties >> 

        public BaseStorage<DeepRLAgentParameters> Agents { get; set; }
        public BaseStorage<QNetworkParameters> QNetworks { get; set; }
        public BaseStorage<StockExchangeParameters> Stocks{ get; set; }        
        public BaseStorage<Experience> Experiences { get; set; }
        public BaseStorage<Period> Periods{ get; set; }
        public BaseStorage<State> States{ get; set; }
        public BaseStorage<SimulationResult> SimulationResults{ get; set; }
        public BaseStorage<OnDayComplete> OnDaysCompleted { get; set; }

        public BaseStorage<AverageTrueRange> AverageTrueRange { get; set; }
        public BaseStorage<BollingerBandsPercentB> BollingerBandsPercentB { get; set; }
        public BaseStorage<DMI> DMI{ get; set; }
        public BaseStorage<SimpleMovingAverage> SimpleMovingAverage { get; set; }
        public BaseStorage<ExponentialMovingAverage> ExponentialMovingAverage { get; set; }
        public BaseStorage<MACD> MACD { get; set; }
        public BaseStorage<RSI> RSI{ get; set; }

        #endregion

        #region << Private Properties >>

        public SQLiteConnection Connection { get; set; }

        #endregion

        #region << Constructor >> 

        public DatabaseContext()
        {
            var platform = new SQLite.Net.Platform.Generic.SQLitePlatformGeneric();
            Connection = new SQLiteConnection(platform, "deppQStock");
                        
            Connection.CreateTable<DeepRLAgentParameters>();
            Connection.CreateTable<QNetworkParameters>();
            Connection.CreateTable<StockExchangeParameters>();
            Connection.CreateTable<Experience>();
            Connection.CreateTable<Period>();
            Connection.CreateTable<State>();
            Connection.CreateTable<SimulationResult>();
            Connection.CreateTable<OnDayComplete>();

            Connection.CreateTable<AverageTrueRange>();
            Connection.CreateTable<BollingerBandsPercentB>();
            Connection.CreateTable<DMI>();
            Connection.CreateTable<SimpleMovingAverage>();
            Connection.CreateTable<ExponentialMovingAverage>();
            Connection.CreateTable<MACD>();
            Connection.CreateTable<RSI>();

            Agents = new BaseStorage<DeepRLAgentParameters>(platform, Connection);
            QNetworks = new BaseStorage<QNetworkParameters>(platform, Connection);
            Stocks = new BaseStorage<StockExchangeParameters>(platform, Connection);
            Experiences = new BaseStorage<Experience>(platform, Connection);
            Periods = new BaseStorage<Period>(platform, Connection);
            States = new BaseStorage<State>(platform, Connection);
            SimulationResults = new BaseStorage<SimulationResult>(platform, Connection);
            OnDaysCompleted = new BaseStorage<OnDayComplete>(platform, Connection);

            AverageTrueRange = new BaseStorage<AverageTrueRange>(platform, Connection);
            BollingerBandsPercentB = new BaseStorage<BollingerBandsPercentB>(platform, Connection);
            DMI = new BaseStorage<DMI>(platform, Connection);
            SimpleMovingAverage = new BaseStorage<SimpleMovingAverage>(platform, Connection);
            ExponentialMovingAverage = new BaseStorage<ExponentialMovingAverage>(platform, Connection);
            MACD = new BaseStorage<MACD>(platform, Connection);
            RSI = new BaseStorage<RSI>(platform, Connection);
        }

        /// <summary>
        /// Removes the agent.
        /// </summary>
        public void RemoveAgent(DeepRLAgentParameters agent)
        {
            //var decisions = OnDaysCompleted.GetAll().Where(d => d.AgentId == agent.Id);
            //if (decisions.Count() > 0)
            //{
            //    OnDaysCompleted.Delete(decisions);
            //}

            //var currentState = States.GetById(agent.StockExchange.CurrentStateId);

            //States.Delete(currentState);

            //var indicatorsIds = Indicators.Where(i => i.StockExchangeId == agent.StockExchangeId).ToList();
            //Indicators.Delete(indicatorsIds);

            //Stocks.Delete(agent.StockExchange);

            //QNetworks.Delete(agent.QNetwork);

            ////TODO: Remove the simulation results.

            //Agents.Delete(agent);
        }

        #endregion
    }
}
