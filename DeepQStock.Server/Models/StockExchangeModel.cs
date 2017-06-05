using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Models
{
    public class StockExchangeModel
    {
        public long id { get; set; }
        public AgentModel Agent { get; set; }
        public int EpisodeLength { get; set; }
        public int NumberOfPeriods { get; set; }
        public int InitialCapital { get; set; }
        public double TransactionCost { get; set; }
        public double SimulationVelocity { get; set; }
    }
}
