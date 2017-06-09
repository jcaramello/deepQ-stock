using DeepQStock.Agents;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;

namespace DeepQStock.Server.Hubs
{
    public class StockExchangeHub : Hub
    {
        #region << Public Properties >>       

        /// <summary>
        /// Agent Storage
        /// </summary>
        public BaseStorage<StockExchangeParameters> StockExchangeStorage { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StockExchangeHub(BaseStorage<StockExchangeParameters> stockExchangeStorage)
        {           
            StockExchangeStorage = stockExchangeStorage;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Save a stock
        /// </summary>
        /// <param name="stock"></param>
        public long Save(StockExchangeParameters stock)
        {            
            StockExchangeStorage.Save(stock);
            stock.CsvDataFilePath = string.Format("{0}/{1}.csv", Settings.CsvDataDirectory, stock.Symbol);
            return stock.Id;
        }

        #endregion
    }
}
