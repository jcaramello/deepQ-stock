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

        /// <summary>
        /// The Type name
        /// </summary>
        private string TypeName { get; set; }

        /// <summary>
        /// Key Name Generator
        /// </summary>                       
        private string SequenceTpl
        {
            get { return string.Format("key:{0}", TypeName); }
        }

        /// <summary>
        /// Index key template
        /// </summary>
        private string IndexTpl
        {
            get { return string.Format("index:{0}", TypeName); }
        }

        private string KeyEntityTpl
        {
            get { return string.Format("entity:{0}", TypeName); }
        }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage" /> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public BaseStorage(IConnectionMultiplexer redis)
        {
            Database = redis.GetDatabase();
            TypeName = typeof(T).Name;
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
                return string.Format("{0}:{1}", KeyEntityTpl, id.Value);
            }
            else
            {
                return KeyEntityTpl;
            }
        }

        /// <summary>
        /// Gets the next sequence.
        /// </summary>
        /// <returns></returns>
        protected long GetNextSequence()
        {
            if (Database.KeyExists(SequenceTpl))
            {
                Database.StringIncrement(SequenceTpl);
            }
            else
            {
                Database.StringSet(SequenceTpl, 1);
            }

            var id = long.Parse(Database.StringGet(SequenceTpl));            

            Database.SortedSetAdd(IndexTpl, GetKey(id), id);

            return id;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns></returns>
        protected RedisKey[] GetKeys()
        {            
            return Database.SortedSetRangeByScore(IndexTpl).Select(s => (RedisKey)s.ToString()).ToArray();
        }

        #endregion

        #region << IStorage Members >>

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            var keys = GetKeys();
            if (keys.Length > 0)
            {
                return Database.StringGet(keys).Select(v => JsonConvert.DeserializeObject<T>(v));
            }
            else
            {
                return new List<T>();
            }
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
            var key = GetKey(model.Id);

            Database.SortedSetRemove(IndexTpl, key);
            Database.KeyDelete(key);
        }

        #endregion
    }
}
