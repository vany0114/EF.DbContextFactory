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
            string rootPath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            GrantAccess(Path.Combine(rootPath, "App_Data", "EF.DbContextFactory.IntegrationTest.Ninject.mdf"));
            AppDomain.CurrentDomain.SetData(
                "DataDirectory",
                Path.Combine(rootPath, "App_Data"));

            NinjectWebCommon.Start();
        }

        [TestMethod]
        public async Task Ninject_add_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            var orderManager = new OrderManager(repo);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                var orders = new List<Order>();
                await orderManager.Create(out orders);
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
            await Assert.ThrowsExceptionAsync<EntityCommandExecutionException>(async () =>
            {
                await orderManager.Delete(orders);
            });
        }

        private static void ResetDataBase()
        {
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            repo.DeleteAll();
        }

        /// <summary>
        /// In order to give write acces because on CI all files are readonly.
        /// </summary>
        /// <param name="fullPath">Database path.</param>
        private static void GrantAccess(string fullPath)
        {
            FileInfo fileInfo = new FileInfo(fullPath);
            fileInfo.IsReadOnly = false;
        }
    }
}
