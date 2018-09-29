using System;
using System.Collections.Generic;
using System.Data.Entity;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NSubstitute;

namespace EF.DbContextFactory.UnitTest.Data
{
    [TestCategory("Factory")]
    [TestClass]    
    public class OrderRepositoryTest
    {
        private readonly Mock<DbSet<Order>> _mockOrderSet = new Mock<DbSet<Order>>();
        private readonly Mock<OrderContext> _mockContext = new Mock<OrderContext>();
        private readonly Mock<Func<OrderContext>> _mockFactory = new Mock<Func<OrderContext>>();

        [TestMethod]
        [Description("Basic test scenarios to test the creation of the repository passing a factory instead the context instance")]
        public void Create_new_order()
        {
            _mockContext.Setup(m => m.Orders).Returns(_mockOrderSet.Object);
            _mockFactory.Setup(x => x()).Returns(_mockContext.Object);

            var orderId = Guid.NewGuid();
            var newOrder = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {orderId}",
                Id = orderId,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var repository = new OrderRepositoryWithFactory(_mockFactory.Object);
            repository.Add(newOrder);

            _mockOrderSet.Verify(m => m.Add(It.IsAny<Order>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [Description("Basic test scenarios to test the creation of the repository passing a factory instead the context instance")]
        public void Get_all_orders()
        {
            var order1Id = Guid.NewGuid();
            var orderItem1Order1 = new OrderItem { Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000, OrderId = order1Id};
            var orderItem2Order1 = new OrderItem { Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000, OrderId = order1Id };

            var order1 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order 1 {order1Id}",
                Id = order1Id,
                OrderItems = new List<OrderItem> {orderItem1Order1, orderItem2Order1}
            };

            var order2Id = Guid.NewGuid();
            var orderItem1Order2 = new OrderItem { Id = Guid.NewGuid(), Name = "Item A", Quantity = 1, UnitPrice = 1000, OrderId = order2Id };

            var order2 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order 2 {order2Id}",
                Id = order2Id,
                OrderItems = new List<OrderItem> {orderItem1Order2}
            };

            var order3Id = Guid.NewGuid();
            var orderItem1Order3 = new OrderItem { Id = Guid.NewGuid(), Name = "Item B", Quantity = 1, UnitPrice = 1000, OrderId = order3Id };
            var orderItem2Order3 = new OrderItem { Id = Guid.NewGuid(), Name = "Item C", Quantity = 4, UnitPrice = 5000, OrderId = order3Id };
            var orderItem3Order3 = new OrderItem { Id = Guid.NewGuid(), Name = "Item D", Quantity = 4, UnitPrice = 5000, OrderId = order3Id };
            var order3 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order 3 {order3Id}",
                Id = order3Id,
                OrderItems = new List<OrderItem> {orderItem1Order3, orderItem2Order3, orderItem3Order3}
            };

            var data = new List<Order>
            {
                order1,
                order2,
                order3,
            }.AsQueryable();

            // Moq doesn't work for static methods (like Include), so it's necessary bypass it via Substitute.
            var mockOrderSet = Substitute.For<DbSet<Order>, IQueryable<Order>>();
            ((IQueryable<Order>)mockOrderSet).Provider.Returns(data.Provider);
            ((IQueryable<Order>)mockOrderSet).Expression.Returns(data.Expression);
            ((IQueryable<Order>)mockOrderSet).ElementType.Returns(data.ElementType);
            ((IQueryable<Order>)mockOrderSet).GetEnumerator().Returns(data.GetEnumerator());
            mockOrderSet.Include(x => x.OrderItems).Returns(mockOrderSet);

            _mockContext.Setup(m => m.Orders).Returns(mockOrderSet);
            _mockFactory.Setup(x => x()).Returns(_mockContext.Object);

            var repository = new OrderRepositoryWithFactory(_mockFactory.Object);
            var orders = repository.GetAllOrders().ToList();

            Assert.AreEqual(3, orders.Count);
            Assert.AreEqual($"Order 1 {order1Id}", orders[0].Description);
            Assert.AreEqual($"Order 2 {order2Id}", orders[1].Description);
            Assert.AreEqual($"Order 3 {order3Id}", orders[2].Description);
            Assert.AreEqual(2, orders[0].OrderItems.Count);
            Assert.AreEqual(1, orders[1].OrderItems.Count);
            Assert.AreEqual(3, orders[2].OrderItems.Count);
        }
    }
}
