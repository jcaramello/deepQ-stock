using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{
    public class CircularQueue<T> : Queue<T>
    {
        #region << Private Members >>         

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        private int Capacity { get; set; }

        #endregion

        #region << Public Properties >>

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full.
        /// </summary>
        public bool IsFull { get { return Count == Capacity; } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full.
        /// </summary>
        public bool IsEmpty { get { return Count == 0; } }

        #endregion

        #region << Constructor >> 

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularQueue{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public CircularQueue(int capacity)
            :base(capacity)
        {
            Capacity = capacity;
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// Enqueues the specified elem.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        public new T Enqueue(T elem)
        {
            T dequeueItem = default(T);
            if (IsFull)
            {
                dequeueItem = base.Dequeue();
            }

            base.Enqueue(elem);

            return dequeueItem;
        }

        /// <summary>
        /// Dequeues an element
        /// </summary>
        /// <returns></returns>
        public new T Dequeue()
        {
            return base.Dequeue();
        }

        /// <summary>
        /// Dequeues an element
        /// </summary>
        /// <returns></returns>
        public new T Peek()
        {
            return base.Peek();
        }

        #endregion    

    }
}
