using DeepQStock.DeppRLAgent;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using ServiceStack.Redis;
using System;
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
            var companyName = "";
            var episodeLength = 5;
            var manager = new BasicRedisClientManager("localhost:6379");
            var periodStorage = new PeriodStorage(manager);
            var agent = new DeepRLAgent();

            var stock = new StockExchange(agent, null, p =>
            {
                p.EpisodeLength = episodeLength;
            });

            var mainSectionLine = 10;
            var statusBarLine = 35;

            stock.OnDayComplete += (e, a) =>
            {
                ClearLine(mainSectionLine);

                System.Console.WriteLine(" Compañia {0}", companyName);
                DrawLine();
                System.Console.WriteLine(" Periodo: ");
                System.Console.WriteLine(" Fecha {0} - Open: {1:C} - High: {2:C} - Low: {3:C} - Close: {4:C}", a.Date.ToShortDateString(), a.Period.Open, a.Period.High, a.Period.Low, a.Period.Close);
                System.Console.WriteLine();
                DrawLine();
                System.Console.WriteLine(" Indicadores Bursatiles: ");
                System.Console.WriteLine();
                a.Period.ToString().Split('|').ToList().ForEach(i => System.Console.WriteLine("\t" + i.Trim()));
                System.Console.WriteLine();
                DrawLine();
                System.Console.WriteLine(" Estado del Agente - Dia {0}", a.DayNumber);
                System.Console.WriteLine();
                System.Console.WriteLine(" Accion: {0}\t Recompenza: {1:C}\t", a.SelectedAction, a.Reward);
                System.Console.WriteLine(" Capital Actual: {0:C}\t Cantidad de Acciones: {1}\t Ganancia Acumulada: {2:C}", a.Period.CurrentCapital, a.Period.ActualPosicion, a.AccumulatedProfit);
            };

            agent.OnTrainingEpochComplete += (e, a) =>
            {
                ClearLine(statusBarLine);
                DrawLine();
                System.Console.WriteLine(" Q Network: ");
                System.Console.WriteLine(" Epoch #{0} - Error: {1}", a.Epoch, a.Error.ToString("N5"));
                System.Console.WriteLine();
            };

            var continueSimulated = false;
            companyName = args != null && args.Length > 0 ? args[0] : "APPL";

            do
            {
                stock.DataProvider = new CsvDataProvider(string.Format("../Data/{0}.csv", companyName), episodeLength);

                System.Console.Clear();

                DrawLine('#');
                DrawLine('#');
                DrawHeader("                Universidad Nacional Del Sur                ", '#');
                DrawHeader("      Proyecto Final 2017 - Jose Caramello - LU: 83767      ", '#');
                DrawHeader("                      DeepQ Stock                           ", '#');
                DrawLine('#');
                DrawLine('#');

                var simulationTask = Task.Run(() => stock.Start());

                simulationTask.Wait();


                System.Console.Write("Simulacion Finalizada, Presione una tecla para finalizar o ingrese otro papel : ");
                var previousCompany = companyName;
                companyName = System.Console.ReadLine();
                continueSimulated = !string.IsNullOrEmpty(companyName) && companyName != previousCompany;
            } while (continueSimulated);
        }

        #region << Private Methods >>

        /// <summary>
        /// Clears the line.
        /// </summary>
        /// <param name="line">The line.</param>
        static void ClearLine(int line)
        {
            System.Console.SetCursorPosition(0, line);
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="c">The c.</param>
        static void DrawLine(char c = '_')
        {
            var line = new string(c, System.Console.WindowWidth);
            System.Console.WriteLine(line);
        }

        static void DrawHeader(string label, char c = '_')
        {
            var half = label.Length / 2;
            var middle = System.Console.WindowWidth / 2;
            var lenght = middle - half;
            var left = new string(c, lenght);
            var right = new string(c, lenght);

            System.Console.WriteLine(string.Format("{0}{1}{2}", left, label, right));
        }

        #endregion
    }
}
