using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EF.DbContextFactory.Examples.Data.Entity;

namespace EF.DbContextFactory.Examples.Data.Repository
{
    public interface IOrderRepository
    {
        void Add(Order order);

        IEnumerable<Order> GetAllOrders();

        Order GetOrderById(Guid id);

        void DeleteById(Guid id);

        Task<int> Update(Order order);

        void DeleteAll();
    }
}