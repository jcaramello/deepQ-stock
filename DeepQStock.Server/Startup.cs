using DeepQStock.Server.Models;
using DeepQStock.Server.Resolvers;
using DeepQStock.Storage;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            app.UseWebApi(config);
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }

        public void ConfigureService(IServiceCollection services)
        {
            services.AddSingleton<IRedisClientsManager, BasicRedisClientManager>(provider => new BasicRedisClientManager("localhost:6379"));            
        }
    }
}
