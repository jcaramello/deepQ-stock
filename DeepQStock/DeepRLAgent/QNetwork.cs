using DeepQStock.Domain;
using DeepQStock.Enums;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System;
using System.Collections.Generic;
using System.IO;

namespace DeepQStock.DeppRLAgent
{
    /// <summary>
    /// Implement a Q state-action function using an internal neural network
    /// </summary>
    public class QNetwork
    {

        #region << Private Properties >>

        /// <summary>
        /// Gets or sets the internal neural network.
        /// </summary>
        private BasicNetwork NeuralNetwork { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        private QNetworkParameters Parameters { get; set; }

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Occurs when [on training epoch complete].
        /// </summary>
        public event EventHandler<OnTrainingEpochCompleteArgs> OnTrainingEpochComplete;

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Initializes a new instance of the <see cref="QNetwork"/> class.
        /// </summary>
        public QNetwork(Action<QNetworkParameters> initializer = null)
        {
            Parameters = new QNetworkParameters();
            initializer?.Invoke(Parameters);
            InitializeNetwork();
        }

        public QNetwork(string path)
        {
            Parameters = new QNetworkParameters();
            FileInfo networkFile = new FileInfo(string.Format(@"{0}\QNetwork.eg", path));
            NeuralNetwork = (BasicNetwork)(Encog.Persist.EncogDirectoryPersistence.LoadObject(networkFile));

        }

        #endregion

        #region << Indexer >>

        /// <summary>
        /// Gets the <see cref="QNetwork"/> with the specified state.
        /// </summary>
        /// <value>
        /// The <see cref="QNetwork"/>.
        /// </value>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public IDictionary<ActionType, double> this[State state]
        {
            get
            {
                var input = new BasicMLData(state.ToArray());
                var result = NeuralNetwork.Compute(input);
                return new Dictionary<ActionType, double>()
                {
                    {ActionType.Buy, result[0] },
                    {ActionType.Sell, result[1] },
                    {ActionType.Wait, result[2] }
                };
            }
        }

        #endregion

        #region << Public Methods >> 

        /// <summary>
        /// Trains the neural network with the passed in training set.        
        /// Receive a list of tuple, where each tuple represent
        /// Tuple = (State, ExpectedValueForBuy, ExpectedValueForSell, ExpectedValueForWait)
        /// </summary>
        /// <param name="trainingSet">The training set.</param>
        public void Train(IList<Tuple<State, double[]>> trainingSet)
        {
            var trainingData = new List<IMLDataPair>();

            foreach (var sample in trainingSet)
            {
                var flattenState = sample.Item1.ToArray();
                var data = new BasicMLData(flattenState);
                var results = new BasicMLData(sample.Item2);

                trainingData.Add(new BasicMLDataPair(data, results));
            }

            IMLDataSet dataSet = new BasicMLDataSet(trainingData);
            //IMLTrain train = new Backpropagation(NeuralNetwork, dataSet, Parameters.LearningRate, Parameters.LearningMomemtum);
            IMLTrain train = new ResilientPropagation(NeuralNetwork, dataSet);

            int epoch = 1;
            do
            {
                train.Iteration();
                epoch++;

                OnTrainingEpochComplete?.Invoke(this, new OnTrainingEpochCompleteArgs()
                {
                    Epoch = epoch,
                    Error = train.Error
                });

            } while (train.Error > Parameters.TrainingError && epoch < Parameters.MaxIterationPerTrainging);
        }

        public void Save(string path)
        {
            FileInfo networkFile = new FileInfo(string.Format(@"{0}\QNetwork.eg", path));
            Encog.Persist.EncogDirectoryPersistence.SaveObject(networkFile, (BasicNetwork)NeuralNetwork);
        }

        #endregion

        #region << Private Methods >>

        /// <summary>
        /// Initializes this neural network.
        /// </summary>
        private void InitializeNetwork()
        {
            NeuralNetwork = new BasicNetwork();

            foreach (var l in Parameters.Layers)
            {
                NeuralNetwork.AddLayer(new BasicLayer(l.ActivationFunction, l.Bias, l.NueronCount));
            }

            NeuralNetwork.Structure.FinalizeStructure();
            NeuralNetwork.Reset();
        }

        #endregion 
    }
}
