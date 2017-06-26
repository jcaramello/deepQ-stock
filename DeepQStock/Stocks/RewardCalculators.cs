using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Stocks
{
    public static class RewardCalculators
    {
        /// <summary>
        /// Private properties
        /// </summary>
        private static CircularQueue<double> Winnings = new CircularQueue<double>(14);
        private static CircularQueue<double> Loosings = new CircularQueue<double>(14);        

        /// <summary>
        /// Return the earning obtained
        /// remarks: Best performance but big error
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        public static double Earnings(StockExchange stock)
        {
            return stock.Earnings - stock.TransactionCost;
        }

        /// <summary>
        /// Return Earnings increment over net capital.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        public static double EarningOverNetCapital(StockExchange stock)
        {
            return (stock.Earnings - stock.TransactionCost) / stock.NetCapital;
        }


        /// <summary>
        /// Winnningses the over loosings.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        public static double WinningsOverLoosings(StockExchange stock)
        {
            var netEarnings = stock.Earnings - stock.TransactionCost;
            var reward = 0.0;

            if (netEarnings > 0)
            {
                Winnings.Enqueue(netEarnings);
            }
            else
            {
                Loosings.Enqueue(netEarnings);
            }

            var totalWinnings = Winnings.ToList().Sum();
            var totalLoosings = Math.Abs(Loosings.ToList().Sum());

            if (totalLoosings > 0)
            {
                reward = totalWinnings / totalLoosings;
            }
            else
            {
                reward = totalWinnings;
            }

            return Math.Round(reward, 2);
        }

        /// <summary>
        /// Winnningses the over loosings.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        public static double AnnualRent(StockExchange stock)
        {
            return stock.AnnualRent;
        }

    }
}
