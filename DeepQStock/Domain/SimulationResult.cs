using DeepQStock.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Domain
{
    public class SimulationResult : BaseModel
    {
        public long agentId { get; set; }
        public string Symbol { get; set; }
        public double AnnualProfits { get; set; }
        public double AnnualRent { get; set; }
        public double Profits { get; set; }
        public double Earnings { get; set; }
        public double NetCapital { get; set; }
        public double TransactionCost { get; set; }
       
    }
}
