using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Models
{
    public class QNetwork : BaseModel
    {
        public int HiddenLayersCount { get; set; }
        public int NeuronCountForHiddenLayers { get; set; }
        public string ActivationFcForHiddenLayers { get; set; }
        public string ActivationFcForOuputLayer { get; set; }

    }
}
