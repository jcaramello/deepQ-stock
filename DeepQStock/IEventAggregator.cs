using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock
{
    /// <summary>
    /// IEventAggregator interface.
    /// This interface defines what basic functionalities are provided by the EventAggregator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// This method registers the specified handler, and associates
        /// it with the specified event type T.
        /// </summary>
        /// <param name="handler">
        /// The EventHandler method.
        /// </param>
        /// <typeparam name="T">
        /// The concrete type that implements IEvent.
        /// </typeparam>
        void Register<T>(EventHandler<T> handler) where T : EventArgs;


        /// <summary>
        /// This method triggers the specified concrete event.
        /// This will call all type-associated handler delegates and pass
        /// evt as a parameter to them.
        /// </summary>
        /// <param name="evt">
        /// The event object that implements IEvent.
        /// </param>
        void Trigger(object sender, EventArgs evt);
    }

}
