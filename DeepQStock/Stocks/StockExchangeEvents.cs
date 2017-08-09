using DeepQStock.Agents;
using DeepQStock.Domain;
using DeepQStock.Enums;
using DeepQStock.Storage;
using Newtonsoft.Json;
using System;

namespace DeepQStock.Stocks
{
    public class OnEpisodeComplete : BaseModel
    {
        public double Reward { get; set; }
    }

    public class OnSimulationComplete : BaseModel
    {
        /// <summary>
        /// Gets or sets the agent identifier.
        /// </summary>
        public long AgentId { get; set; }
    }

    public class OnDayComplete : BaseModel
    {
        /// <summary>
        /// Gets or sets the agent identifier.
        /// </summary>        
        public long AgentId { get; set; }

        /// <summary>
        /// Gets or sets the day number.
        /// </summary>        
        public int DayNumber { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the selected action.
        /// </summary>
        public ActionType SelectedAction { get; set; }

        /// <summary>
        /// Gets or sets the reward.
        /// </summary>
        public double Reward { get; set; }

        /// <summary>
        /// Gets or sets the accumulated profit.
        /// </summary>
        public double AccumulatedProfit { get; set; }

        /// <summary>
        /// Gets or sets the annual rent.
        /// </summary>
        public double AnnualRent { get; set; }

        /// <summary>
        /// Gets or sets the annual profits.
        /// </summary>
        public double AnnualProfits { get; set; }

        /// <summary>
        /// Gets or sets the total of years.
        /// </summary>
        public int TotalOfYears { get; set; }

        /// <summary>
        /// Gets or sets the actual position.
        /// </summary>                       
        public Period Period { get; set; }        
    }
}
