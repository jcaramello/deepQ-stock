using DeepQStock.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Domain
{
    public class Experience
    {
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        [JsonIgnore]
        public State From { get; set; }

        /// <summary>
        /// State From id
        /// </summary>
        public long FromId { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public ActionType Action { get; set; }

        /// <summary>
        /// Gets or sets the reward.
        /// </summary>
        public double Reward { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        [JsonIgnore]
        public State To { get; set; }

        /// <summary>
        /// State To Id
        /// </summary>
        public long ToId { get; set; }
    }
}
