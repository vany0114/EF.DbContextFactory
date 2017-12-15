using System;
using System.Data.Entity;
using System.Web;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using EF.DbContextFactory.Ninject.Extensions;

namespace EF.DbContextFactory.IntegrationTest.Ninject
{
    public static class NinjectWebCommon
    {
        public static IKernel Kernel;
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
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
                RegisterServices(kernel);
                Kernel = kernel;
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
            kernel.Bind<IOrderRepository>().To<OrderRepository>();

            // with DbContextFactory
            kernel.AddDbContextFactory<OrderContext>();
            kernel.Bind<IOrderRepository>().To<OrderRepositoryWithFactory>();
        }
    }
}
