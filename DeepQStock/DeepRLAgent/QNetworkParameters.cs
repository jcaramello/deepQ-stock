using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.DeppRLAgent
{

    public class LayerParameters
    {
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerParameters"/> class.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="bias">if set to <c>true</c> [bias].</param>
        /// <param name="neuronCount">The neuron count.</param>        
        public LayerParameters(IActivationFunction activationFunction, bool bias, int neuronCount)
        {
            ActivationFunction = activationFunction;
            Bias = bias;
            NueronCount = neuronCount;            
        }

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LayerParameters"/> is bias.
        /// </summary>
        public bool Bias { get; set; }

        /// <summary>
        /// Gets or sets the nuerons count.
        /// </summary>
        public int NueronCount { get; set; }

        /// <summary>
        /// Gets or sets the activation function.
        /// </summary>
        public IActivationFunction ActivationFunction { get; set; }

        #endregion
    }


    public class QNetworkParameters
    {

        #region << Constructor >>

        public QNetworkParameters()
        {
            Layers = new List<LayerParameters>();
            TrainingError = 0.00001;
            MaxIterationPerTrainging = 20;
        }

        #endregion


        #region << Public Properties >>

        /// <summary>
        /// Gets or sets the learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Gets or sets the learning momemtum.
        /// </summary>
        public double LearningMomemtum { get; set; }

        /// <summary>
        /// Gets or sets the layers.
        /// </summary>
        public IList<LayerParameters> Layers{ get; set; }

        /// <summary>
        /// Gets or sets the training error.
        /// </summary>
        public double TrainingError { get; set; }

        /// <summary>
        /// Gets or sets the maximum iteration per trainging.
        /// </summary>
        public int MaxIterationPerTrainging { get; set; }

        #endregion
    }
}
