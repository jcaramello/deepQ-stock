using DeepQStock.Indicators;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class TechnicalIndicatorStorage : BaseStorage<TechnicalIndicatorBase>
    {

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="TechnicalIndicatorStorage"/> class.
        /// </summary>
        /// <param name="conn">The connection.</param>
        public TechnicalIndicatorStorage(IConnectionMultiplexer conn) : base(conn)
        {
        }

        #endregion

        #region << IStorage Members >>       

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TechnicalIndicatorBase> GetAll()
        {
            var keys = GetKeys();
            if (keys.Length > 0)
            {
                return Database.StringGet(keys).Select(v => 
                {
                    var indicator = JsonConvert.DeserializeObject<TechnicalIndicatorBase>(v);
                    var concreteType = Type.GetType(indicator.ClassType);
                    indicator = (TechnicalIndicatorBase)JsonConvert.DeserializeObject(v, concreteType);

                    return indicator;
                });
            }
            else
            {
                return new List<TechnicalIndicatorBase>();
            }
        }

        #endregion
    }
}
