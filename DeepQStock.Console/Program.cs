using DeepQStock.DeppRLAgent;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using ServiceStack.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeepQStock.Console
{
    public class Program
    {
        public static int StartLine = System.Console.WindowTop;
        public static int MainSectionLine = StartLine + 7;
        public static int StatusBarLine = StartLine + 35;

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            var parsed = CommandLine.Parser.Default.ParseArguments(args, options);

            if (parsed && !string.IsNullOrEmpty(options.InputFile))
            {
                var companyName = "";
                var status = "";
                var episodeLength = 5;
                var manager = new BasicRedisClientManager("localhost:6379");
                var periodStorage = new PeriodStorage(manager);
                var initialCapital = 100000;
                int? firstYear = null;
                int? actualYear = null;
                int totalYears = 1;

                var agent = new DeepRLAgent(p =>
                {
                    p.DiscountFactor = options.DiscountFactor > 0 ? options.DiscountFactor : p.DiscountFactor;
                    p.eGreedyProbability = options.eGreedyProbability > 0 ? options.eGreedyProbability : p.eGreedyProbability;
                    p.MiniBatchSize = options.MiniBatchSize > 0 ? options.MiniBatchSize : p.MiniBatchSize;
                    p.MemoryReplaySize = options.MemoryReplaySize > 0 ? options.MemoryReplaySize : p.MemoryReplaySize;

                });

                var stock = new StockExchange(agent, null, p =>
                {
                    p.EpisodeLength = episodeLength;
                    p.InitialCapital = initialCapital;
                    p.SimulationVelocity = options.Velocity > 0 ? (int)(options.Velocity * 1000.0) : p.SimulationVelocity;
                    p.TransactionCost = options.TransactionCost > 0 ? options.TransactionCost : p.TransactionCost;
                });

                stock.OnDayComplete += (e, a) =>
                {
                    firstYear = firstYear == null ? a.Period.Date.Year : firstYear;
                    actualYear = a.Period.Date.Year;
                    totalYears = (actualYear.Value - firstYear.Value) + 1;

                    DrawSummarySection(companyName, status, totalYears, initialCapital, a);
                };                

                agent.OnTrainingEpochComplete += DrawStatusBar;

                var continueSimulated = false;
                companyName = options.InputFile;

                do
                {
                    stock.DataProvider = new CsvDataProvider(companyName, episodeLength);
                    DateTime endTraining = DateTime.Today;

                    if (options.TrainingPhase > 0)
                    {
                        var firstDay = stock.DataProvider.GetAll().ToList().Select(s => s.Date).Min();
                        endTraining = firstDay.AddYears(options.TrainingPhase);

                        stock.DataProvider.Seek(endDate: endTraining);
                        status = "Etapa: Entrenamiento";
                    }


                    System.Console.Clear();
                    System.Console.SetCursorPosition(0, 0);
                    DrawHeaderSection();

                    var stockTask = Task.Run(() => stock.Start());
                    stockTask.Wait();

                    if (options.TrainingPhase > 0)
                    {
                        status = "Etapa: Entrenamiento Completado";
                        WriteLine(StatusBarLine + 5, "Etapa de entrenamiento Completada, Presione una tecla para con la etapa de evaluacion... ");
                        System.Console.ReadLine();
                        ClearCurrentLine();

                        status = "Etapa: Evaluacion";

                        stock.CurrentState = null;
                        stock.DataProvider.Seek(startDate: endTraining);
                        agent.Save(@"./");

                        agent = new DeepRLAgent(p =>
                        {
                            p.DiscountFactor = options.DiscountFactor > 0 ? options.DiscountFactor : p.DiscountFactor;
                            p.eGreedyProbability = options.eGreedyProbability > 0 ? options.eGreedyProbability : p.eGreedyProbability;
                            p.MiniBatchSize = options.MiniBatchSize > 0 ? options.MiniBatchSize : p.MiniBatchSize;
                            p.MemoryReplaySize = options.MemoryReplaySize > 0 ? options.MemoryReplaySize : p.MemoryReplaySize;
                        });


                        agent.NetworkPath = "./";
                        stock.Agent = agent;

                        stockTask = Task.Run(() => stock.Start());
                        stockTask.Wait();
                    }

                    WriteLine(StatusBarLine + 5, "Simulacion Finalizada, Presione una tecla para finalizar o ingrese otro papel : ");
                    var previousCompany = companyName;
                    companyName = System.Console.ReadLine();
                    continueSimulated = !string.IsNullOrEmpty(companyName) && companyName != previousCompany;

                    stock.CurrentState = null;

                } while (continueSimulated);
            }
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
            ClearCurrentLine();
            System.Console.SetCursorPosition(0, line);
            System.Console.Write(text, args);
        }

        /// <summary>
        /// Clears the line.
        /// </summary>
        /// <param name="line">The line.</param>
        static void ClearCurrentLine()
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

        /// <summary>
        /// Draw header
        /// </summary>
        /// <param name="line"></param>
        /// <param name="label"></param>
        /// <param name="c"></param>
        static void DrawHeader(int line, string label, char c = '_')
        {
            System.Console.SetCursorPosition(0, line);
            var half = label.Length / 2;
            var middle = System.Console.WindowWidth / 2;
            var lenght = middle - half;
            var left = new string(c, lenght);
            var right = new string(c, lenght + 1);

            System.Console.Write(string.Format("{0}{1}{2}", left, label, right));
        }

        /// <summary>
        /// Draw Header Section
        /// </summary>
        static void DrawHeaderSection()
        {
            DrawLine(StartLine, '#');
            DrawHeader(StartLine + 1, "                Universidad Nacional Del Sur                ", '#');
            DrawHeader(StartLine + 2, "      Proyecto Final 2017 - Jose Caramello - LU: 83767      ", '#');
            DrawHeader(StartLine + 3, "                      DeepQ Stock                           ", '#');
            DrawLine(StartLine + 4, '#');
            DrawLine(StartLine + 5, '#');
        }

        /// <summary>
        /// Draw the summury section
        /// </summary>
        /// <param name="mainSectionLine"></param>
        /// <param name="companyName"></param>
        /// <param name="totalYears"></param>
        /// <param name="initialCapital"></param>
        /// <param name="a"></param>
        static void DrawSummarySection(string companyName, string status, int totalYears, int initialCapital, OnDayCompleteArgs a)
        {

            WriteLine(MainSectionLine, " Simbolo {0} - {1}", companyName, status);
            DrawLine(MainSectionLine + 1);
            WriteLine(MainSectionLine + 2, " Periodo: ");
            WriteLine(MainSectionLine + 3, " Fecha {0} - Open: {1:C} - High: {2:C} - Low: {3:C} - Close: {4:C}", a.Date.ToShortDateString(), a.Period.Open, a.Period.High, a.Period.Low, a.Period.Close);
            WriteLine(MainSectionLine + 4);
            DrawLine(MainSectionLine + 5);
            WriteLine(MainSectionLine + 6, " Indicadores Bursatiles: ");
            WriteLine(MainSectionLine + 7);

            var indicatorsSection = MainSectionLine + 8;
            var indicators = a.Period.ToString().Split('|');

            for (int i = 0; i < indicators.Length; i++)
            {
                var val = indicators[i];
                WriteLine(indicatorsSection + i, "\t" + val.Trim());
            }

            var agentSummary = indicatorsSection + indicators.Length + 1;

            WriteLine(agentSummary);
            DrawLine(agentSummary + 1);
            WriteLine(agentSummary + 2, " Estado del Agente - Año {0} - Dia {1}", totalYears, a.DayNumber);
            WriteLine(agentSummary + 3);
            WriteLine(agentSummary + 4, " Accion: {0}\t Recompenza: {1:N5}\t", a.SelectedAction, a.Reward);

            var annualProfits = a.AccumulatedProfit / totalYears;
            WriteLine(agentSummary + 5, " Capital Actual: {0:C}\t Cantidad de Acciones: {1}\t Ganancia Acumulada: {2:C}\t Renta Anual: {3:P2}",
                a.Period.CurrentCapital, a.Period.ActualPosicion, a.AccumulatedProfit, annualProfits / initialCapital);
        }

        /// <summary>
        /// Draw Status Bar
        /// </summary>
        /// <param name="e"></param>
        /// <param name="a"></param>
        static void DrawStatusBar(object e, OnTrainingEpochCompleteArgs a)
        {
            DrawLine(StatusBarLine);
            WriteLine(StatusBarLine + 2, " Q Network: Training Epoch #{0} - Error: {1}", a.Epoch, a.Error.ToString("N9"));
            WriteLine(StatusBarLine + 3);
        }



        #endregion
    }
}
