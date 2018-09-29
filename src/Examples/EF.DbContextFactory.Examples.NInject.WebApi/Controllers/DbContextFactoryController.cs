using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.NInject.WebApi.Models;

namespace EF.DbContextFactory.Examples.NInject.WebApi.Controllers
{
    [RoutePrefix("api/factory")]
    public class DbContextFactoryController : ApiController
    {
        private readonly IOrderRepository _orderRepository;

        public DbContextFactoryController(IOrderRepository orderRepository)
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
        public IHttpActionResult Post()
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
        public IHttpActionResult Delete(List<Guid> ordersToDelete)
        {
            Parallel.ForEach(ordersToDelete, order =>
            {
                _orderRepository.DeleteById(order);
            });

            return Ok();
        }

        [Route("updateorders")]
        // DELETE api/values/5
        public async Task<IHttpActionResult> Put()
        {
            var ordersToUpdate = _orderRepository.GetAllOrders();
            var tasks = new List<Task<int>>();

            foreach (var order in ordersToUpdate)
            {
                order.Date = DateTime.Now;
                order.Description += "...Updated";
                tasks.Add(Task.Run(() => _orderRepository.Update(order)));
            }

            await Task.WhenAll(tasks);

            return Ok();
        }
    }
}
