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
        public RedisContext Context { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StockExchangeHub(RedisContext ctx)
        {           
            Context = ctx;
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
            Context.StockExchanges.Save(stock);
            
            return stock.Id;
        }

        /// <summary>
        /// Gets an stock exchange by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public StockExchangeParameters GetById(long id)
        {
            return Context.StockExchanges.GetById(id);
        }

        #endregion
    }
}
