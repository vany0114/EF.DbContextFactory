using System.Web.Http;
using WebApi.StructureMap;

namespace EF.DbContextFactory.Examples.StructureMap41.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.UseStructureMap<CustomRegistry>();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
