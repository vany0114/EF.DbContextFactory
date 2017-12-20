using EFCore.DbContextFactory.Examples.Data.Entity;
using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCore.DbContextFactory.IntegrationTest
{
    [Trait("Category", "EFCore")]
    public class EFCoreTests
    {
        private readonly TestServer _server;

        public EFCoreTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
        }

        [Fact(DisplayName ="EFCore_add_orders_without_EF_DbContextFactory")]
        public async Task EFCore_add_orders_without_EF_DbContextFactory()
        {
            var repo = (OrderRepository)_server.Host.Services.GetService(typeof(OrderRepository));
            var orderManager = new OrderManager(repo);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                try
                {
                    var orders = new List<Order>();
                    await orderManager.Create(out orders);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Entity framework thread safe exception", ex);
                }
            });
        }

        [Fact(DisplayName = "EFCore_add_orders_with_EF_DbContextFactory")]
        public void EFCore_add_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            var task = orderManager.Create(out orders);
            task.Wait();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(3, repo.GetAllOrders().Count());
        }

        [Fact(DisplayName = "EFCore_delete_orders_with_EF_DbContextFactory")]
        public async Task EFCore_delete_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            await orderManager.Create(out orders);
            var task = orderManager.Delete(orders);
            task.Wait();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(0, repo.GetAllOrders().Count());
        }

        [Fact(DisplayName = "EFCore_delete_orders_without_EF_DbContextFactory")]
        public async Task EFCore_delete_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var repo = (OrderRepository)_server.Host.Services.GetService(typeof(OrderRepository));
            var repoWithFactory = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);
            var orderManagerWithFactory = new OrderManager(repoWithFactory);

            var orders = new List<Order>();
            await orderManagerWithFactory.Create(out orders);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                try
                {
                    await orderManager.Delete(orders);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Entity framework thread safe exception", ex);
                }
            });
        }

        private void ResetDataBase()
        {
            var repo = (OrderRepository)_server.Host.Services.GetService(typeof(OrderRepository));
            repo.DeleteAll();
        }
    }
}
