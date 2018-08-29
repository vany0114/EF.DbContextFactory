﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using WebApi.StructureMap;

namespace EF.DbContextFactory.IntegrationTest.StructureMap41.WebApi
{
    [TestCategory("StructureMap.WebApi")]
    [TestClass]
    public class StructureMapTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            GlobalConfiguration.Configuration.UseStructureMap<CustomRegistry>();
        }

        [TestMethod]
        public async Task StructureMapWebApi_add_orders_without_EF_DbContextFactory()
        {
            var container = new Container();
            var repo = container.GetInstance<OrderRepository>();
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
        public void StructureMapWebApi_add_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var container = new Container();
            var repo = container.GetInstance<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            var task = orderManager.Create(out orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(3, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task StructureMapWebApi_delete_orders_with_EF_DbContextFactory()
        {
            ResetDataBase();
            var container = new Container();
            var repo = container.GetInstance<OrderRepositoryWithFactory>();
            var orderManager = new OrderManager(repo);

            var orders = new List<Order>();
            await orderManager.Create(out orders);
            var task = orderManager.Delete(orders);
            task.Wait();

            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreEqual(0, repo.GetAllOrders().Count());
        }

        [TestMethod]
        public async Task StructureMapWebApi_delete_orders_without_EF_DbContextFactory()
        {
            ResetDataBase();
            var container = new Container();
            var repo = container.GetInstance<OrderRepository>();
            var repoWithFactory = container.GetInstance<OrderRepositoryWithFactory>();
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
            var container = new Container();
            var repo = container.GetInstance<OrderRepository>();
            repo.DeleteAll();
        }
    }
}
