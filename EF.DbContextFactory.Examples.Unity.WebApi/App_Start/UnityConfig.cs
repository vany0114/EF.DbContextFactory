using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Unity.Extensions;
using System.Data.Entity;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace EF.DbContextFactory.Examples.Unity.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // without DbContextFactory
            container.RegisterType<DbContext, OrderContext>(new TransientLifetimeManager());
            container.RegisterType<IOrderRepository, OrderRepository>("NoFactory", new TransientLifetimeManager());

            // with DbContextFactory
            container.AddDbContextFactory<OrderContext>();
            container.RegisterType<IOrderRepository, OrderRepositoryWithFactory>("WithFactory", new TransientLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}