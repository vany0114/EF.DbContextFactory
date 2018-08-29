// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EF.DbContextFactory.IntegrationTest.StructureMap.DependencyResolution {
    using EF.DbContextFactory.Examples.Data.Persistence;
    using EF.DbContextFactory.Examples.Data.Repository;
    using EF.DbContextFactory.StructureMap.Extensions;
    using global::StructureMap.Configuration.DSL;
    using global::StructureMap.Graph;
    using global::StructureMap.Web.Pipeline;
    using System.Data.Entity;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {

            // without DbContextFactory
            For<DbContext>().Use<OrderContext>();
            For<IOrderRepository>().Use<OrderRepository>();

            // with DbContextFactory
            this.AddDbContextFactory<OrderContext>();
            For<IOrderRepository>().Use<OrderRepositoryWithFactory>();
        }

        #endregion
    }
}