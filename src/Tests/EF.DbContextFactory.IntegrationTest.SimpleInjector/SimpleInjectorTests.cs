using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.SimpleInjector.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace EF.DbContextFactory.IntegrationTest.SimpleInjector
{
    [TestCategory("SimpleInjector")]
    [TestClass]
    public class SimpleInjectorTests
    {
        public static Container Container;

        private Scope _scope;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            Container = new Container();

            Container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // without DbContextFactory
            Container.Register<OrderContext>(Lifestyle.Scoped);
            Container.Register<OrderRepository>(Lifestyle.Transient);

            // with DbContextFactory
            Container.AddDbContextFactory<OrderContext>();
            Container.Register<OrderRepositoryWithFactory>(Lifestyle.Scoped);

            Container.Verify();
        }

        [TestInitialize]
        public void Initialize()
        {
            _scope = ThreadScopedLifestyle.BeginScope(Container);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope.Dispose();
        }

        [TestMethod]
        [Ignore]
        public async Task SimpleInjector_add_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = Container.GetInstance<OrderRepository>();
            var orderManager = new OrderManager(repo);

            await Assert.ThrowsExceptionAsync<EntityException>(async () =>
            {
                try
                {
                    var orders = new List<Order>();
                    await orderManager.Create(out orders);
                }
                catch (Exception ex)
                {
                    throw new EntityException("Entity framework thread safe exception", ex);
                }
            });
        }

        [TestMethod]
        public void SimpleInjector_add_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = Container.GetInstance<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            var task = orderManager.Create(out orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(3, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task SimpleInjector_delete_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = Container.GetInstance<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            await orderManager.Create(out orders);
            var task = orderManager.Delete(orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(0, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task SimpleInjector_delete_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = Container.GetInstance<OrderRepository>();
            var repoWithFactory = Container.GetInstance<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);
            var orderManagerWithFactory = new OrderManager(repoWithFactory);

            var orders = new List<Order>();
            await orderManagerWithFactory.Create(out orders);
            await Assert.ThrowsExceptionAsync<EntityException>(async () =>
            {
                try
                {
                    await orderManager.Delete(orders);
                }
                catch (Exception ex)
                {
                    throw new EntityException("Entity framework thread safe exception", ex);
                }
            });
        }

        private static void ResetDataBase()
        {
            var repo = Container.GetInstance<OrderRepository>();
            repo.DeleteAll();
        }
    }
}
