using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Utils
{

    /// <summary>
    /// EventAggregator class.
    /// This class represents a single threaded IEventAggregator implementation
    /// that uses a minimum of reflection and the EventHandler generic delegate type.
    /// For more information see IEventAggregator.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {

        IDictionary<Type, IList<EventHandler<EventArgs>>> handlers;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EventAggregator()
        {
            handlers = new Dictionary<Type, IList<EventHandler<EventArgs>>>();
        }

        /// <summary>
        /// Register a listerner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Register<T>(EventHandler<T> handler) where T : EventArgs
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers[typeof(T)] = new List<EventHandler<EventArgs>>();
            }

            var handlerList = handlers[typeof(T)];
            handlerList.Add((s, e) => handler(s, (T)e));
        }

        /// <summary>
        /// Trigger
        /// </summary>
        /// <param name="evt"></param>
        public void Trigger(object sender, EventArgs evt)
        {
            IList<EventHandler<EventArgs>> handlerList;

            if (handlers.TryGetValue(evt.GetType(), out handlerList))
            {
                foreach (EventHandler<EventArgs> handler in handlerList)
                {
                    handler.Invoke(sender, evt);
                }
            }
        }
    }
}