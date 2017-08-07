using DeepQStock.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    public class ExperienceStorage : BaseStorage<Experience>
    {
        #region << Private >>         

        /// <summary>
        /// Gets or sets the period storage.
        /// </summary>
        private StateStorage StateStorage { get; set; }

        #endregion

        #region << Constructor >>

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStorage"/> class.
        /// </summary>
        /// <param name="conn">The CTX.</param>
        public ExperienceStorage(IConnectionMultiplexer conn) : base(conn)
        {
            StateStorage = new StateStorage(conn);
        }

        #endregion

        #region << IStorage Members >>       

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Experience> GetAll()
        {
            return base.GetAll().Select(e =>
            {
                e.From = StateStorage.GetById(e.FromId);
                e.To = StateStorage.GetById(e.ToId);

                return e;
            });
        }

        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override Experience GetById(long id)
        {
            var experience = base.GetById(id);

            if (experience != null)
            {
                experience.From = StateStorage.GetById(experience.FromId);
                experience.To = StateStorage.GetById(experience.ToId);

            }

            return experience;
        }

        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <param name="item">The item.</param>        
        public override void Save(Experience item)
        {
            StateStorage.Save(item.From);
            StateStorage.Save(item.To);

            item.FromId = item.From.Id;
            item.ToId = item.To.Id;

            base.Save(item);
        }

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Delete(Experience item)
        {
            if (item == null) return;

            StateStorage.DeleteByIds(new List<long> { item.FromId, item.ToId });

            base.Delete(item);
        }

        #endregion

        #region << Private Methods >>


        #endregion

    }
}
