using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace EF.DbContextFactory.IntegrationTest.Ninject
{
    [TestCategory("Ninject")]
    [TestClass]
    public class NinjectTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            NinjectWebCommon.Start();
        }

        [TestMethod]
        public async Task Ninject_add_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            var orderManager = new OrderManager(repo);

            await Assert.ThrowsExceptionAsync<EntityException>(async () =>
            {
                try
                {
                    var orders = new List<Order>();
                    await orderManager.Create(out orders);
                }
                catch(Exception ex)
                {
                    throw new EntityException("Entity framework thread safe exception", ex);
                }
            });
        }

        [TestMethod]
        public void Ninject_add_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            var task = orderManager.Create(out orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(3, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task Ninject_delete_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            await orderManager.Create(out orders);
            var task = orderManager.Delete(orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(0, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task Ninject_delete_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            var repoWithFactory = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
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
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            repo.DeleteAll();
        }
    }
}
