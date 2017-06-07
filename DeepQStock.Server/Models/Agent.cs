using DeepQStock.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Models
{
    public class Agent : BaseModel
    {               
        public string Name { get; set; }

        public string Symbol { get; set; }

        public double EGreedy { get; set; }

        public double InOutStrategy { get; set; }

        public int MiniBatchSize { get; set; }

        public double DiscountFactor { get; set; }

        public int HiddenLayersCount { get; set; }

        public int NeuronCountForLayer { get; set; }

        public int MemoryReplaySize { get; set; }
        
        public QNetwork QNetwork { get; set; }

        public long QNetworkId{ get; set; }

    }
}
