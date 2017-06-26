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
        public StorageManager Manager { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StockExchangeHub(StorageManager manager)
        {           
            Manager = manager;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Save a stock
        /// </summary>
        /// <param name="stock"></param>
        public long Save(StockExchangeParameters stock)
        {
            stock.CsvDataFilePath = string.Format("{0}\\{1}.csv", Settings.CsvDataDirectory, stock.Symbol);
            Manager.StockExchangeStorage.Save(stock);
            
            return stock.Id;
        }

        /// <summary>
        /// Gets an stock exchange by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public StockExchangeParameters GetById(long id)
        {
            return Manager.StockExchangeStorage.GetById(id);
        }

        #endregion
    }
}
