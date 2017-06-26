using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class BaseStorage<T> : IStorage<T> where T : BaseModel
    {

        #region << Protected Properties >>  

        /// <summary>
        /// Gets or sets the redis manager.
        /// </summary>
        private IDatabase Database { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage" /> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public BaseStorage(IConnectionMultiplexer redis)
        {
            Database = redis.GetDatabase();
        }

        #endregion

        #region << Protected Members >>

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        protected string GetKey(long? id = null)
        {
            if (id.HasValue)
            {
                return string.Format("entity:{0}:{1}", typeof(T).Name, id.Value);
            }
            else
            {
                return string.Format("entity:{0}", typeof(T).Name);
            }
        }

        /// <summary>
        /// Gets the next sequence.
        /// </summary>
        /// <returns></returns>
        protected long GetNextSequence()
        {
            var typeName = typeof(T).Name;
            var keyGenerator = string.Format("key:{0}", typeName);

            if (Database.KeyExists(keyGenerator))
            {
                Database.StringIncrement(keyGenerator);
            }
            else
            {
                Database.StringSet(keyGenerator, 1);
            }

            var id = long.Parse(Database.StringGet(keyGenerator));

            var index = string.Format("index:{0}", typeName);
            var key = string.Format("key:{0}:{1}", typeName, id);

            Database.SortedSetAdd(index, key, id);

            return id;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetKeys()
        {
            var typeName = typeof(T).Name;
            var keyGenerator = string.Format("key:{0}", typeName);
            var index = string.Format("index:{0}", typeName);

            return Database.SortedSetRangeByScore(index).Select(s => s.ToString());
        }

        #endregion

        #region << IStorage Members >>

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            //var keys =
            //var s = Database.StringGetRange(GetKey(), 0, -1).Select(v => JsonConvert.DeserializeObject<T>(v));
            //return s;
            return null;
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual T GetById(long id)
        {
            return JsonConvert.DeserializeObject<T>(Database.StringGet(GetKey(id)));
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetByIds(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public virtual void Save(T model)
        {
            if (model.Id == 0)
            {
                model.Id = GetNextSequence();
            }

            var serialized = JsonConvert.SerializeObject(model);
            Database.StringSet(GetKey(model.Id), serialized);
        }


        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="model">The item.</param>
        public virtual void Delete(T model)
        {
            Database.KeyDelete(GetKey(model.Id));
        }

        #endregion
    }
}
