using DeepQStock;
using DeepQStock.Config;
using DeepQStock.Storage;
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
            var episodeLength = 5;
            var storage = new RedisContext();
            var agent = new DeepRLAgent(storage);
            var dataProvider = new CsvDataProvider("../../../data/APPL.csv", episodeLength);
            var stock = new StockExchange(agent, dataProvider, p => 
            {
                p.EpisodeLength = episodeLength;
            });
            
            stock.Start();                   
        }
    }
}
