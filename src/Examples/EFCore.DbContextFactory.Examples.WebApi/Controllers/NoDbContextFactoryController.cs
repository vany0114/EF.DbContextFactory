using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.DbContextFactory.Examples.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using EFCore.DbContextFactory.Examples.Data.Repository;
using EFCore.DbContextFactory.Examples.WebApi.Models;

namespace EFCore.DbContextFactory.Examples.WebApi.Controllers
{
    [Route("api/nofactory")]
    public class NoDbContextFactoryController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public NoDbContextFactoryController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [Route("getallorders")]
        // GET api/values
        public IEnumerable<OrderViewModel> Get()
        {
            var model = _orderRepository.GetAllOrders().Select(x => new OrderViewModel
            {
                Id = x.Id,
                Date = x.Date,
                Description = x.Description
            });

            return model.ToList();
        }

        [Route("getorder")]
        // GET api/values/5
        public OrderViewModel Get(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            return new OrderViewModel
            {
                Id = order.Id,
                Date = order.Date,
                Description = order.Description
            };
        }

        [Route("createorder")]
        // POST api/values
        public IActionResult Post()
        {
            var order1Id = Guid.NewGuid();
            var newOrder1 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order1Id}",
                Id = order1Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var order2Id = Guid.NewGuid();
            var newOrder2 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order2Id}",
                Id = order2Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var order3Id = Guid.NewGuid();
            var newOrder3 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order3Id}",
                Id = order3Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var thread1 = new Thread(() =>
            {
                _orderRepository.Add(newOrder2);
            });

            var thread2 = new Thread(() =>
            {
                _orderRepository.Add(newOrder3);
            });

            var thread3 = new Thread(() =>
            {
                _orderRepository.Add(newOrder1);
            });

            thread1.Start();
            thread2.Start();
            thread3.Start();

            return Ok();
        }

        [Route("deleteorder")]
        // DELETE api/values/5
        public IActionResult Delete([FromBody]List<Guid> ordersToDelete)
        {
            Parallel.ForEach(ordersToDelete, order =>
            {
                _orderRepository.DeleteById(order);
            });

            return Ok();
        }

        [Route("updateorders")]
        // DELETE api/values/5
        public IActionResult Put()
        {
            var ordersToUpdate = _orderRepository.GetAllOrders();

            Parallel.ForEach(ordersToUpdate, async order =>
            {
                order.Date = DateTime.Now;
                order.Description += "...Updated";
                await _orderRepository.Update(order);
            });

            return Ok();
        }
    }
}
