using DeepQStock.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Models
{
    public class StockExchange : BaseModel
    {
        public long AgentId { get; set; }
        
        public Agent Agent { get; set; }

        public int EpisodeLength { get; set; }

        public int NumberOfPeriods { get; set; }

        public int InitialCapital { get; set; }

        public double TransactionCost { get; set; }

        public double SimulationVelocity { get; set; }
    }
}
