using System.Data.Entity;
using System.Web.Http;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.Unity.WebApi.Controllers;

namespace EF.DbContextFactory.Examples.Unity.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
