using DeepQStock.Agents;
using DeepQStock.Server.Hubs;
using DeepQStock.Server.Middleware;
using DeepQStock.Server.Resolvers;
using DeepQStock.Stocks;
using DeepQStock.Storage;
using DeepQStock.Utils;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Owin;
using ServiceStack.Redis;
using System.Linq;
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
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            ConfigureService();

        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureService()
        {
            var redisClientManager = new BasicRedisClientManager(Settings.RedisConnectionString);
            var qNetworkStorage = new BaseStorage<QNetworkParameters>(redisClientManager);
            var agentStorage = new BaseStorage<DeepRLAgentParameters>(redisClientManager);
            var stockExchangeStorage = new BaseStorage<StockExchangeParameters>(redisClientManager);

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);

            var eventAggregator = new EventAggregator();

            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            GlobalHost.DependencyResolver.Register(typeof(IEventAggregator), () => eventAggregator));

            GlobalHost.DependencyResolver.Register(typeof(AgentHub), () => new AgentHub(agentStorage, qNetworkStorage, stockExchangeStorage));
            GlobalHost.DependencyResolver.Register(typeof(StockExchangeHub), () => new StockExchangeHub(stockExchangeStorage));            

        }
    }
}
