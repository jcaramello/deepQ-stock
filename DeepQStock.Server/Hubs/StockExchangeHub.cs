using DeepQStock.Server.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Hubs
{
    public class StockExchangeHub : Hub
    {

        #region << Public Properties >>
        #endregion

        #region << Constructor >>

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StockExchangeHub()
        {
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Save a stock
        /// </summary>
        /// <param name="stock"></param>
        public void Save(StockExchangeModel stock)
        {

        }

        #endregion
    }
}
