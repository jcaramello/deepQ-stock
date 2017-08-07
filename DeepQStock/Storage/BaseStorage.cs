using SQLite.Net;
using System.Collections.Generic;
using System.Linq;

namespace DeepQStock.Storage
{
    public class BaseStorage<T> : IStorage<T> where T : BaseModel, new()
    {
        #region << Protected Properties >>  

        /// <summary>
        /// Gets or sets the redis manager.
        /// </summary>
        protected SQLiteConnection Database { get; set; }        

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodStorage" /> class.
        /// </summary>
        /// <param name="redis">The redis.</param>
        public BaseStorage(SQLiteConnection db)
        {
            Database = db;            
        }

        #endregion       

        #region << IStorage Members >>

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return Database.Table<T>().AsEnumerable();
        }

        /// <summary>
        /// Get all instance of agents
        /// </summary>
        /// <returns></returns>
        public virtual T GetById(long id)
        {
            return Database.Table<T>().SingleOrDefault(i => i.Id == id);

        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetByIds(IEnumerable<long> ids)
        {
            return Database.Table<T>().Where(i => ids.ToArray().Contains(i.Id));
        }

        /// <summary>
        /// Create a new instance of an agent
        /// </summary>
        /// <param name="name"></param>
        public virtual void Save(T model)
        {
            if (model.Id == 0)
            {
                Database.Insert(model);
            }
            else
            {
                Database.Update(model);
            }            
        }


        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="model">The item.</param>
        public virtual void Delete(T model)
        {
            Database.Delete(model);
        }

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="model">The item.</param>
        public virtual void Delete(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Database.Delete(item);
            }           
        }

        #endregion
    }
}
