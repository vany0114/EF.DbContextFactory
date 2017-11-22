using System;
using System.Data.Entity;
using System.Web;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.NInject;
using EF.DbContextFactory.Examples.NInject.Controllers;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using EF.DbContextFactory.Ninject.Extensions;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]
namespace EF.DbContextFactory.Examples.NInject
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // without DbContextFactory
            kernel.Bind<DbContext>().To<OrderContext>().InTransientScope();
            kernel.Bind<IOrderRepository>().To<OrderRepository>()
                .WhenInjectedInto(typeof(NoDbContextFactoryController))
                .InRequestScope();

            // with DbContextFactory
            kernel.AddDbContextFactory<OrderContext>();
            kernel.Bind<IOrderRepository>().To<OrderRepositoryWithFactory>()
                .WhenInjectedInto(typeof(HomeController))
                .InRequestScope();
        }
    }
}