using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFCore.DbContextFactory.Examples.Data.Entity;

namespace EFCore.DbContextFactory.Examples.Data.Repository
{
    public interface IOrderRepository
    {
        void Add(Order order);

        IEnumerable<Order> GetAllOrders();

        Order GetOrderById(Guid id);

        void DeleteById(Guid id);

        Task Update(Order order);

        void DeleteAll();
    }
}