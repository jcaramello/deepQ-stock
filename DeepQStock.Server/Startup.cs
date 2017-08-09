using DeepQStock.Agents;
using DeepQStock.Server.Hubs;
using DeepQStock.Server.Middleware;
using DeepQStock.Server.Resolvers;
using DeepQStock.Server.Utils;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Owin;
using StackExchange.Redis;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace DeepQStock.Server
{
    public class Startup
    {
        /// <summary>
        /// Configure app
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DeepQStockApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
         
            app.Use<GlobalExceptionMiddleware>();    
            app.UseWebApi(config);
            app.UseCors(CorsOptions.AllowAll);

            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            app.MapSignalR(hubConfiguration);

            GlobalConfiguration.Configuration.UseRedisStorage(Settings.RedisConnectionString);
            GlobalConfiguration.Configuration.UseActivator(new DependencyResolverJobActivator(GlobalHost.DependencyResolver));

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            ConfigureService();

            int minWorker, minIOC;            
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMinThreads(8, 8);

        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureService()
        {
            var redis = ConnectionMultiplexer.Connect(Settings.RedisConnectionString);            
            var manager = new RedisManager(redis);            

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);                       

            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            var agentHub = new AgentHub(manager);            

            //Register Hubs
            GlobalHost.DependencyResolver.Register(typeof(AgentHub), () => agentHub);            
            GlobalHost.DependencyResolver.Register(typeof(StockExchange), () => new StockExchange(manager));

        }
    }
}
