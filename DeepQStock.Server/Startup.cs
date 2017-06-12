using DeepQStock.Agents;
using DeepQStock.Server.Hubs;
using DeepQStock.Server.Middleware;
using DeepQStock.Server.Resolvers;
using DeepQStock.Server.Utils;
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
            GlobalConfiguration.Configuration.UseActivator(new DependencyResolverJobActivator(GlobalHost.DependencyResolver));

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
            var redisManager = new BasicRedisClientManager(Settings.RedisConnectionString);            
            var qNetworkStorage = new BaseStorage<QNetworkParameters>(redisManager);
            var agentStorage = new BaseStorage<DeepRLAgentParameters>(redisManager);
            var stockExchangeStorage = new BaseStorage<StockExchangeParameters>(redisManager);

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);                       

            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            var agentHub = new AgentHub(redisManager, agentStorage, qNetworkStorage, stockExchangeStorage);
            var stockExchangeHub = new StockExchangeHub(stockExchangeStorage);

            //Register Hubs
            GlobalHost.DependencyResolver.Register(typeof(AgentHub), () => agentHub);
            GlobalHost.DependencyResolver.Register(typeof(StockExchangeHub), () => stockExchangeHub);
            GlobalHost.DependencyResolver.Register(typeof(StockExchange), () => new StockExchange(redisManager));

        }
    }
}
