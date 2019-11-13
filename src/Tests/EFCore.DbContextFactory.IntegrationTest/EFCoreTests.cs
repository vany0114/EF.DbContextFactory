using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
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

        [Fact(DisplayName ="EFCore_add_orders_without_EF_DbContextFactory_should_throw_an_exception")]
        public async Task EFCore_add_orders_without_EF_DbContextFactory_should_throw_an_exception()
        {
            var repo = (OrderRepository)_server.Host.Services.GetService(typeof(OrderRepository));
            var orderManager = new OrderManager(repo);
            ResetDataBase(repo);

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await orderManager.Create(out _);
            });
        }

        [Fact(DisplayName = "EFCore_add_orders_with_EF_DbContextFactory_should_save_orders_simultaneously")]
        public async Task EFCore_add_orders_with_EF_DbContextFactory_should_save_orders_simultaneously()
        {
            var repo = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);
            ResetDataBase(repo);

            await orderManager.Create(out _);

            Assert.Equal(3, repo.GetAllOrders().Count());
        }

        [Fact(DisplayName = "EFCore_delete_orders_with_EF_DbContextFactory_should_delete_orders_simultaneously")]
        public async Task EFCore_delete_orders_with_EF_DbContextFactory_should_delete_orders_simultaneously()
        {
            var repo = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);
            ResetDataBase(repo);

            await orderManager.Create(out var orders);
            await orderManager.Delete(orders);

            Assert.Empty(repo.GetAllOrders());
        }

        [Fact(DisplayName = "EFCore_delete_orders_without_EF_DbContextFactory_should_throw_an_exception")]
        public async Task EFCore_delete_orders_without_EF_DbContextFactory_should_throw_an_exception()
        {
            var repo = (OrderRepository)_server.Host.Services.GetService(typeof(OrderRepository));
            var repoWithFactory = (OrderRepositoryWithFactory)_server.Host.Services.GetService(typeof(OrderRepositoryWithFactory));
            var orderManager = new OrderManager(repo);
            var orderManagerWithFactory = new OrderManager(repoWithFactory);
            ResetDataBase(repo);

            await orderManagerWithFactory.Create(out var orders);
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await orderManager.Delete(orders);
            });
        }

        private void ResetDataBase(IOrderRepository repo)
        {
            repo.DeleteAll();
        }
    }
}
