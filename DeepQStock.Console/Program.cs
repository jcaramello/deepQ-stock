using DeepQStock.Agents;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using Hangfire;
using Newtonsoft.Json;
using StackExchange.Redis;
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
        public static double initialValue = 0.0;

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
                var redis = ConnectionMultiplexer.Connect("localhost:6379");
                var manager = new RedisManager(redis);
                var context = new DatabaseContext();
                var initialCapital = 100000;
                var dayNumber = 0;
                int totalTrainingDays = 0;

                var agentParameters = new DeepRLAgentParameters();
                agentParameters.DiscountFactor = options.DiscountFactor > 0 ? options.DiscountFactor : agentParameters.DiscountFactor;
                agentParameters.eGreedyProbability = options.eGreedyProbability > 0 ? options.eGreedyProbability : agentParameters.eGreedyProbability;
                agentParameters.MiniBatchSize = options.MiniBatchSize > 0 ? options.MiniBatchSize : agentParameters.MiniBatchSize;
                agentParameters.MemoryReplaySize = options.MemoryReplaySize > 0 ? options.MemoryReplaySize : agentParameters.MemoryReplaySize;
                agentParameters.QNetwork.MaxIterationPerTrainging = options.MaxIterationPerTrainging > 0 ? options.MaxIterationPerTrainging : agentParameters.QNetwork.MaxIterationPerTrainging;
                agentParameters.QNetwork.TrainingError = options.TrainingError > 0 ? options.TrainingError : agentParameters.QNetwork.TrainingError;

                var agent = new DeepRLAgent(agentParameters); ;

                var stockParameters = new StockExchangeParameters();
                stockParameters.EpisodeLength = episodeLength;
                stockParameters.InitialCapital = initialCapital;
                stockParameters.SimulationVelocity = options.Velocity > 0 ? (int)(options.Velocity * 1000.0) : stockParameters.SimulationVelocity;
                stockParameters.TransactionCost = options.TransactionCost > 0 ? options.TransactionCost : stockParameters.TransactionCost;
                stockParameters.RewardCalculator = RewardCalculator.Use(RewardCalculatorType.WinningsOverLoosings);

                var stock = new StockExchange(stockParameters, context, manager, agent, null);

                manager.Subscribe(RedisPubSubChannels.OnDayComplete, (channel, msg)=>
                {
                    var a = JsonConvert.DeserializeObject<OnDayComplete>(msg);
                    dayNumber = a.DayNumber - totalTrainingDays;
                    initialValue = initialValue == 0.0 ? a.Period.Open : initialValue;
                    DrawSummarySection(companyName, status, dayNumber, initialCapital, a);
                });
                

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

                    var stockTask = Task.Run(() => stock.Run(new JobCancellationToken(false)));
                    stockTask.Wait();

                    if (options.TrainingPhase > 0)
                    {
                        status = "Etapa: Entrenamiento Completado";
                        WriteLine(StatusBarLine + 5, "Etapa de entrenamiento Completada, Presione una tecla para continuar con la etapa de evaluacion... ");
                        System.Console.ReadLine();
                        ClearCurrentLine();
                        totalTrainingDays = dayNumber;
                        stock.TotalOfYears = 0;

                        status = "Etapa: Evaluacion";

                        stock.CurrentState = null;
                        stock.DataProvider.Seek(startDate: endTraining);
                        agent.Save(context);


                        agentParameters.DiscountFactor = options.DiscountFactor > 0 ? options.DiscountFactor : agentParameters.DiscountFactor;
                        agentParameters.eGreedyProbability = options.eGreedyProbability > 0 ? options.eGreedyProbability : agentParameters.eGreedyProbability;
                        agentParameters.MiniBatchSize = options.MiniBatchSize > 0 ? options.MiniBatchSize : agentParameters.MiniBatchSize;
                        agentParameters.MemoryReplaySize = options.MemoryReplaySize > 0 ? options.MemoryReplaySize : agentParameters.MemoryReplaySize;

                        agent = new DeepRLAgent(agentParameters);                        
                        stock.Agent = agent;

                        stockTask = Task.Run(() => stock.Run(new JobCancellationToken(false)));
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
        static void DrawSummarySection(string companyName, string status, int dayNumber, int initialCapital, OnDayComplete a)
        {

            WriteLine(MainSectionLine, " Simbolo {0} - {1}", companyName, status);
            DrawLine(MainSectionLine + 1);
            WriteLine(MainSectionLine + 2, " Periodo: ");
            WriteLine(MainSectionLine + 3, " Fecha {0} - Open: {1:C} - High: {2:C} - Low: {3:C} - Close: {4:C} \t\t\t Incremento: {5:P2}", a.Date.ToShortDateString(), a.Period.Open, a.Period.High, a.Period.Low, a.Period.Close, (a.Period.Close - initialValue) / initialValue);
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
            WriteLine(agentSummary + 2, " Estado del Agente - Año {0} - Dia {1}", a.TotalOfYears, dayNumber);
            WriteLine(agentSummary + 3);
            WriteLine(agentSummary + 4, " Accion: {0}\t Recompenza: {1:N9}\t", a.SelectedAction, a.Reward);            
            WriteLine(agentSummary + 5, " Capital Actual: {0:C}\t Cantidad de Acciones: {1}\t Ganancia Acumulada: {2:C}\t\t Renta Anual: {3:P2}", a.Period.CurrentCapital, a.Period.ActualPosition, a.AccumulatedProfit, a.AnnualRent);
            
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
