using DeepQStock.Domain;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class PeriodStorage : BaseStorage<Period>
    {
        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage"/> class.
        /// </summary>        
        public PeriodStorage(IRedisClientsManager manager) : base(manager)
        {
        }

        #endregion

    }
}
