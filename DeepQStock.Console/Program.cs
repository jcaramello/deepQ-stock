using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace deepQStock.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var stock = new StockExchange(parameters =>
            {
                parameters.CsvFilePath = "APPL.csv";
            });

            stock.Agent = new Agent();

            stock.Start();

            
        }
    }
}
