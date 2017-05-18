using DeepQStock.DeppRLAgent;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using ServiceStack.Redis;
using System.Threading.Tasks;

namespace DeepQStock.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var episodeLength = 5;
            var manager = new BasicRedisClientManager("localhost:6379");
            var periodStorage = new PeriodStorage(manager);
            var agent = new DeepRLAgent();
            var dataProvider = new CsvDataProvider("../../../data/APPL.csv", episodeLength);                       

            var stock = new StockExchange(agent, dataProvider, p =>
            {
                p.EpisodeLength = episodeLength;
            });

            System.Console.Clear();
            System.Console.WriteLine("*****************************************");
            System.Console.WriteLine("** DeepQ-Stock - Proyecto Final 2017  **");
            System.Console.WriteLine("*****************************************");

            var mainSectionLine = 5;
            var statusBarLine = 15;            

            stock.OnDayComplete += (e, a) => 
            {                
                System.Console.SetCursorPosition(0, mainSectionLine);
                ClearLine();
                System.Console.WriteLine("*** Dia Numero {0} {1} - Accion Seleccionada: {2} - Recompenza: {3} ***", a.DayNumber, a.Date.ToShortDateString(), a.SelectedAction.ToString(), a.Reward, a.AccumulatedProfit);
                System.Console.WriteLine();
                System.Console.WriteLine("***********************");
                System.Console.WriteLine("***Estado del Agente***");
                System.Console.WriteLine("***********************");
                System.Console.WriteLine();
                System.Console.WriteLine("\t Capital Actual: {0}");
                System.Console.WriteLine("\t Cantidad de Acciones: {0}");
                System.Console.WriteLine("\t Ganancia Acumulada: ${4:0.00}: {0}");
                System.Console.WriteLine();
            };

            agent.OnTrainingEpochComplete += (e, a) =>
            {
                System.Console.SetCursorPosition(0, statusBarLine);
                ClearLine();
                System.Console.WriteLine(@"**Iteracion de entrenamiento #{0} - Completada - Error: {1}", a.Epoch, a.Error);
                System.Console.WriteLine();
            };

            stock.Start();
        }

        #region << Private Methods >>

        static void ClearLine()
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        #endregion
    }
}
