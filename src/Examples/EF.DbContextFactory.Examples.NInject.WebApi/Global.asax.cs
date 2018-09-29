using System.Data.Entity;
using System.Web.Http;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.NInject.WebApi.Controllers;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using EF.DbContextFactory.Ninject.Extensions;

namespace EF.DbContextFactory.Examples.NInject.WebApi
{
    public class WebApiApplication : NinjectHttpApplication
    {
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private void RegisterServices(IKernel kernel)
        {
            // without DbContextFactory
            kernel.Bind<DbContext>().To<OrderContext>().InTransientScope();
            kernel.Bind<IOrderRepository>().To<OrderRepository>()
                .WhenInjectedInto(typeof(NoDbContextFactoryController))
                .InRequestScope();

            // with DbContextFactory
            kernel.AddDbContextFactory<OrderContext>();
            kernel.Bind<IOrderRepository>().To<OrderRepositoryWithFactory>()
                .WhenInjectedInto(typeof(DbContextFactoryController))
                .InRequestScope();
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
