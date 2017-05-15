using DeepQStock;
using DeepQStock.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var stock = new StockExchange(parameters =>
            {
                parameters.CsvFilePath = "../../../data/APPL.csv";
                parameters.Agent = new DeepRLAgent();
            });
            
            stock.Start();                   
        }
    }
}
