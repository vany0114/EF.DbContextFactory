using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EF.DbContextFactory.Examples.Data.Persistence;
using EF.DbContextFactory.Examples.Data.Repository;
using StructureMap;
using EF.DbContextFactory.StructureMap.WebApi.Extensions;

namespace EF.DbContextFactory.IntegrationTest.StructureMap41.WebApi
{
    public class CustomRegistry : Registry
    {
        public CustomRegistry()
        {
            // without DbContextFactory
            For<DbContext>().Use<OrderContext>();
            For<IOrderRepository>().Use<OrderRepository>();

            // with DbContextFactory
            this.AddDbContextFactory<OrderContext>();
            For<IOrderRepository>().Use<OrderRepositoryWithFactory>();
        }
    }
}