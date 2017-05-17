using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Storage
{
    /// <summary>
    /// Implement a persistent layer
    /// </summary>
    public interface IStorage<T> where T : class
    {
        /// <summary>
        /// Retrieve an item of type T by the given key from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T GetBy(string key);

        /// <summary>
        /// Gets all item of type T from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll(IEnumerable<string> keys);

        /// <summary>
        /// Saves an item of type T to the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Save(T item);

        /// <summary>
        /// Deletes an item of type T from the storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Delete(T item);
    }
}
