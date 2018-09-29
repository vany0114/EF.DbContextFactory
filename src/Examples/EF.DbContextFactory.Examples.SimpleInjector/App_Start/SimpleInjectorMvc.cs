using System.Reflection;
using System.Web.Mvc;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.SimpleInjector.Extensions;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace EF.DbContextFactory.Examples.SimpleInjector
{
    public static class SimpleInjectorMvc
    {
        public static void Register()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.Register<OrderContext>(Lifestyle.Scoped);
            container.AddDbContextFactory<OrderContext>();

            container.Register<OrderRepository>(Lifestyle.Scoped);
            container.Register<OrderRepositoryWithFactory>(Lifestyle.Scoped);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}