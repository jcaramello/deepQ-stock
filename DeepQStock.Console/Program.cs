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
            var initialCapital = 100000;

            var stock = new StockExchange(agent, null, p =>
            {
                p.EpisodeLength = episodeLength;
                p.InitialCapital = initialCapital;
            });

            var mainSectionLine = 9;
            var statusBarLine = 35;

            stock.OnDayComplete += (e, a) =>
            {
                WriteLine(mainSectionLine, " Compañia {0}", companyName);
                DrawLine(mainSectionLine + 1);
                WriteLine(mainSectionLine + 2, " Periodo: ");
                WriteLine(mainSectionLine + 3, " Fecha {0} - Open: {1:C} - High: {2:C} - Low: {3:C} - Close: {4:C}", a.Date.ToShortDateString(), a.Period.Open, a.Period.High, a.Period.Low, a.Period.Close);
                WriteLine(mainSectionLine + 4);
                DrawLine(mainSectionLine + 5);
                WriteLine(mainSectionLine + 6, " Indicadores Bursatiles: ");
                WriteLine(mainSectionLine + 7);

                var indicatorsSection = mainSectionLine + 8;
                var indicators = a.Period.ToString().Split('|');

                for (int i = 0; i < indicators.Length; i++)
                {
                    var val = indicators[i];
                    WriteLine(indicatorsSection + i, "\t" + val.Trim());
                }

                var agentSummary = indicatorsSection + indicators.Length + 1;

                WriteLine(agentSummary);
                DrawLine(agentSummary + 1);
                WriteLine(agentSummary + 2, " Estado del Agente - Dia {0}", a.DayNumber);
                WriteLine(agentSummary + 3);
                WriteLine(agentSummary + 4, " Accion: {0}\t Recompenza: {1:C}\t", a.SelectedAction, a.Reward);                                
                WriteLine(agentSummary + 5, " Capital Actual: {0:C}\t Cantidad de Acciones: {1}\t Ganancia Acumulada: {2:C}\t Renta Anual: {3:P2}", a.Period.CurrentCapital, a.Period.ActualPosicion, a.AccumulatedProfit, a.AccumulatedProfit / initialCapital);
            };

            agent.OnTrainingEpochComplete += (e, a) =>
            {
                DrawLine(statusBarLine);
                WriteLine(statusBarLine + 1, " Q Network: ");
                WriteLine(statusBarLine + 2, " Epoch #{0} - Error: {1}", a.Epoch, a.Error.ToString("N5"));
                WriteLine(statusBarLine + 3);
            };

            var continueSimulated = false;
            companyName = args != null && args.Length > 0 ? args[0] : "APPL";

            do
            {
                stock.DataProvider = new CsvDataProvider(string.Format("../Data/{0}.csv", companyName), episodeLength);

                System.Console.Clear();
                System.Console.SetCursorPosition(0, 0);

                DrawLine(0, '#');
                DrawLine(1, '#');
                DrawHeader(2, "                Universidad Nacional Del Sur                ", '#');
                DrawHeader(3, "      Proyecto Final 2017 - Jose Caramello - LU: 83767      ", '#');
                DrawHeader(4, "                      DeepQ Stock                           ", '#');
                DrawLine(5, '#');
                DrawLine(6, '#');

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
        /// Writes the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="text">The text.</param>
        /// <param name="args">The arguments.</param>
        static void WriteLine(int line, string text = "", params object[] args)
        {
            System.Console.SetCursorPosition(0, line);
            ClearLine();
            System.Console.SetCursorPosition(0, line);
            System.Console.Write(text, args);
        }

        /// <summary>
        /// Clears the line.
        /// </summary>
        /// <param name="line">The line.</param>
        static void ClearLine()
        {
            System.Console.Write(new string(' ', System.Console.WindowWidth));
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="c">The c.</param>
        static void DrawLine(int line, char c = '_')
        {
            System.Console.SetCursorPosition(0, line);
            var text = new string(c, System.Console.WindowWidth);
            System.Console.Write(text);
        }

        static void DrawHeader(int line, string label, char c = '_')
        {
            System.Console.SetCursorPosition(0, line);
            var half = label.Length / 2;
            var middle = System.Console.WindowWidth / 2;
            var lenght = middle - half;
            var left = new string(c, lenght);
            var right = new string(c, lenght);

            System.Console.Write(string.Format("{0}{1}{2}", left, label, right));
        }

        #endregion
    }
}
