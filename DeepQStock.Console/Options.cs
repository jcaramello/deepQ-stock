using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Console
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Archivo csv con los datos de simulacion. El Archivo debe tener el siguiente formato .")]
        public string InputFile { get; set; }

        [Option('t', "training", DefaultValue = 8, HelpText = "Numero de años a usar para entrenar al agente")]
        public int TrainingPhase { get; set; }

        [Option('e', "egreedy", HelpText = "Probabilidad de seleccion de accion random")]
        public int eGreedyProbability { get; set; }

        [Option('b', "batch", HelpText = "Tamaño del mini batch de entrenamiento")]
        public int MiniBatchSize { get; set; }

        [Option('d', "discount", HelpText = "discount factor")]
        public double DiscountFactor { get; set; }

        [Option('i', "iterations", HelpText = "Numero maximo de iteraciones de entrenamiento")]
        public int MaxIterationPerTrainging { get; set; }

        [Option('c', "tcost", HelpText = "Costo de cada transaccion realizada por el agente, representa un porcentaje del total de la operacion")]
        public double TransactionCost { get; set; }

        [Option('m', "memory", HelpText = "Tamaño de la memory replay del agente")]
        public int MemoryReplaySize { get; set; }

        [Option('v', "velocity", HelpText = "Velocidad de simulacion, representa la cantidad de segundos entre cada dia")]
        public double Velocity { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("DeepQ Stock", "1.0.0"),
                Copyright = new CopyrightInfo("Jose Caramello", DateTime.Today.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = System.Console.WindowWidth,
            };

            help.AddPreOptionsLine(Environment.NewLine);
            help.AddPreOptionsLine("Usage: DeepQStock.Console.exe -f <<file>>.csv");
            help.AddOptions(this);

            var haveError = LastParserState != null && LastParserState.Errors != null && LastParserState.Errors.Count > 0;

            return help;
        }
    }

}
