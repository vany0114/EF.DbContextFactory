using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.Unity.Models;
using Unity.Attributes;

namespace EF.DbContextFactory.Examples.Unity.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public HomeController([Dependency("WithFactory")] IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public ActionResult Index()
        {
            var model = _orderRepository.GetAllOrders().Select(x => new OrderViewModel
            {
                Id = x.Id,
                Date = x.Date,
                Description = x.Description
            });

            return View(model.ToList());
        }

        public ActionResult Create()
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

            // waits 1/2 sec while all orders are inserted in order the view display them.
            Thread.Sleep(500);
            return RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            return View(order);
        }

        [HttpPost]
        public ActionResult Delete(List<OrderViewModel> orders)
        {
            var ordersToDelete = orders.Where(x => x.Selected).ToList();
            Parallel.ForEach(ordersToDelete, order =>
            {
                _orderRepository.DeleteById(order.Id);
            });

            return RedirectToAction("Index");
        }
    }
}