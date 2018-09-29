using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EF.DbContextFactory.Examples.Data.Entity;
using EF.DbContextFactory.Examples.Data.Persistence;

namespace EF.DbContextFactory.Examples.Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void DeleteById(Guid id)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == id);
            _context.Entry(order).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.Include(x => x.OrderItems);
        }

        public Order GetOrderById(Guid id) => _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);

        public async Task<int> Update(Order order)
        {
            _context.Orders.Attach(order);
            _context.Entry(order).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public void DeleteAll()
        {
            var orders = _context.Orders;
            foreach (var order in orders)
            {
                _context.Entry(order).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }
    }
}