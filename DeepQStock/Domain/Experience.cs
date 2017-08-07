using DeepQStock.Enums;
using DeepQStock.Storage;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Domain
{
    public class Experience : BaseModel
    {
        /// <summary>
        /// Agent Id
        /// </summary>
        public long AgentId { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        [OneToMany]
        public State From { get; set; }

        /// <summary>
        /// State From id
        /// </summary>
        [ForeignKey(typeof(State))]
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
        [OneToMany]
        public State To { get; set; }

        /// <summary>
        /// State To Id
        /// </summary>
        [ForeignKey(typeof(State))]
        public long ToId { get; set; }
    }
}
