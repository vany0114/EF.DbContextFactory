using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using EF.DbContextFactory.Examples.StructureMap41.WebApi.Controllers;
using StructureMap;
using EF.DbContextFactory.StructureMap.WebApi.Extensions;

namespace EF.DbContextFactory.Examples.StructureMap41.WebApi
{
    public class CustomRegistry : Registry
    {
        public CustomRegistry()
        {
            var orderRepository = For<IOrderRepository>().Use<OrderRepository>();
            var orderRepositoryWithFactory = For<IOrderRepository>().Use<OrderRepositoryWithFactory>();

            // without DbContextFactory
            For<DbContext>().Use<OrderContext>().Transient();
            For<NoDbContextFactoryController>().Use<NoDbContextFactoryController>()
                .Ctor<IOrderRepository>("orderRepository").Is(orderRepository);

            // with DbContextFactory
            this.AddDbContextFactory<OrderContext>();
            For<DbContextFactoryController>().Use<DbContextFactoryController>()
                .Ctor<IOrderRepository>("orderRepository").Is(orderRepositoryWithFactory);
        }
    }
}