using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class PeriodStorage : IStorage<Period>
    {

        #region << Private Properties >>

        /// <summary>
        /// Redis Context
        /// </summary>
        public RedisContext Context { get; set; }

        #endregion

        #region << Constructor >>

        public PeriodStorage(RedisContext ctx)
        {
            Context = ctx;
        }

        #endregion

        #region << IStorage Members >>

        public void Delete(Period item)
        {
            Context.Client.Remove(item.Key);
        }

        public IEnumerable<Period> GetAll()
        {
            var keys = Context.GetKeys<Period>();
            return Context.Client.GetAll<Period>(keys).Values.ToList();
        }

        public Period GetBy(string key)
        {
            return Context.Client.Get<Period>(key);
        }

        public void Save(Period item)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
