using Hangfire;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQStock.Server.Utils
{
    public class DependencyResolverJobActivator : JobActivator
    {
        private IDependencyResolver _container;

        public DependencyResolverJobActivator(IDependencyResolver container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.GetService(type);
        }
    }
}
