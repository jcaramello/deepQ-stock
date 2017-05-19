using DeepQStock.DeppRLAgent;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using ServiceStack.Redis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeepQStock.Console
{
    public class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
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
            System.Console.WriteLine();
            System.Console.WriteLine("\tUniversidad Nacional Del Sur");
            System.Console.WriteLine("\tProyecto Final 2017");
            System.Console.WriteLine("\tDeepQ-Stock");
            DrawLine('*');

            var mainSectionLine = 6;
            var statusBarLine = 30;

            stock.OnDayComplete += (e, a) =>
            {
                System.Console.SetCursorPosition(0, mainSectionLine);
                ClearLine();                
                System.Console.WriteLine(" Periodo: ");
                DrawLine();
                System.Console.WriteLine(" Fecha {0} - Open: {1:C} - High: {2:C} - Low: {3:C} - Close: {4:C}", a.Date.ToShortDateString(), a.Period.Open, a.Period.High, a.Period.Low, a.Period.Close);
                System.Console.WriteLine();
                System.Console.WriteLine(" Indicadores Bursatiles: ");
                DrawLine();                
                a.Period.ToString().Split('|').ToList().ForEach(i => System.Console.WriteLine(" " + i.Trim()));
                System.Console.WriteLine();                
                System.Console.WriteLine(" Estado del Agente Dia {0}", a.DayNumber);
                DrawLine();
                System.Console.WriteLine();
                System.Console.WriteLine(" Accion: {0}\t Recompenza: {1:C}\t", a.SelectedAction, a.Reward);
                System.Console.WriteLine(" Capital Actual: {0:C}\t Cantidad de Acciones: {1}\t Ganancia Acumulada: {2:C}", a.Period.CurrentCapital, a.Period.ActualPosicion, a.AccumulatedProfit);
            };

            agent.OnTrainingEpochComplete += (e, a) =>
            {
                System.Console.SetCursorPosition(0, statusBarLine);
                ClearLine();                
                System.Console.WriteLine("  Q Network: ");
                DrawLine();
                System.Console.WriteLine(@" Epoch #{0} - Error: {1:0.0000000000}", a.Epoch, a.Error);
                System.Console.WriteLine();
            };

            var simulationTask = Task.Run(() => stock.Start());

            simulationTask.Wait();


            System.Console.Write("Simulacion Finalizada, Presione una tecla para continuar.");
            System.Console.ReadKey();
        }

        #region << Private Methods >>

        static void ClearLine()
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        static void DrawLine(char c = '_')
        {
            var line = new string(c, System.Console.WindowWidth);
            System.Console.Write(line);
        }

        #endregion
    }
}
