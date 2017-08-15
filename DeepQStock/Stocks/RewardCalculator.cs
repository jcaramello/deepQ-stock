using DeepQStock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Stocks
{
    public enum RewardCalculatorType
    {
        Earnings,
        EarningOverNetCapital,
        WinningsOverLoosings,
        AnnualRent
    }

    public class RewardCalculator
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RewardCalculatorType Type { get; set; }

        /// <summary>
        /// Private properties
        /// </summary>
        private CircularQueue<double> Winnings { get; set; }
        private CircularQueue<double> Loosings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RewardCalculator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        private RewardCalculator(RewardCalculatorType type)
        {
            Type = type;
            Winnings = new CircularQueue<double>(14);
            Loosings = new CircularQueue<double>(14);
        }

        /// <summary>
        /// Return the earning obtained
        /// remarks: Best performance but big error
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        private double Earnings(StockExchange stock)
        {
            return stock.Earnings - stock.TransactionCost;
        }

        /// <summary>
        /// Return Earnings increment over net capital.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        private double EarningOverNetCapital(StockExchange stock)
        {
            return (stock.Earnings - stock.TransactionCost) / stock.NetCapital;
        }


        /// <summary>
        /// Winnningses the over loosings.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        private double WinningsOverLoosings(StockExchange stock)
        {            
            var netEarnings = stock.Earnings != 0 ? stock.Earnings - stock.TransactionCost : 0;
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
        private double AnnualRent(StockExchange stock)
        {
            return stock.AnnualRent;
        }

        /// <summary>
        /// Calculates the specified stock.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <returns></returns>
        public double Calculate(StockExchange stock)
        {
            var reward = 0.0;

            switch (Type)
            {
                case RewardCalculatorType.Earnings:
                    reward = Earnings(stock);
                    break;
                case RewardCalculatorType.EarningOverNetCapital:
                    reward = EarningOverNetCapital(stock);
                    break;
                case RewardCalculatorType.WinningsOverLoosings:
                    reward = WinningsOverLoosings(stock);
                    break;
                case RewardCalculatorType.AnnualRent:
                    reward = AnnualRent(stock);
                    break;
                default:
                    break;
            }

            return reward;
        }

        /// <summary>
        /// Uses the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static RewardCalculator Use(RewardCalculatorType type)
        {
            return new RewardCalculator(type);
        }

    }
}
