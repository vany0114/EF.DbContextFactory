using System;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            string rootPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            AppDomain.CurrentDomain.SetData(
                "DataDirectory",
                Path.Combine(rootPath, "App_Data"));

            NinjectWebCommon.Start();
        }

        [TestMethod]
        public async Task Add_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            var orderManager = new OrderManager(repo);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await orderManager.Create(out var orders);
            });
        }

        [TestMethod]
        public void Add_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var task = orderManager.Create(out var orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(3, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task Delete_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            await orderManager.Create(out var orders);
            var task = orderManager.Delete(orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(0, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task Delete_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = NinjectWebCommon.Kernel.Get<OrderRepository>();
            var repoWithFactory = NinjectWebCommon.Kernel.Get<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);
            var orderManagerWithFactory = new OrderManager(repoWithFactory);

            await orderManagerWithFactory.Create(out var orders);
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
    }
}
