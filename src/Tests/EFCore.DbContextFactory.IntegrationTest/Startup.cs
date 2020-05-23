using EFCore.DbContextFactory.Examples.Data.Persistence;
using EFCore.DbContextFactory.Examples.Data.Repository;
using EFCore.DbContextFactory.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EFCore.DbContextFactory.IntegrationTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<OrderContext>(builder =>
                builder.UseInMemoryDatabase("OrdersExample"));

            services.AddDbContextFactory<OrderContext>(
                options => options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)),
                () => SetupConnectionAndBuilderOptions<OrderContext>().ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MigrationAttributeMissingWarning)));

            services.AddScoped<OrderRepositoryWithFactory, OrderRepositoryWithFactory>();
            services.AddTransient<OrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        /// <summary>
        /// With thanks to:
        /// https://www.scottbrady91.com/Entity-Framework/Entity-Framework-Core-In-Memory-Testing
        /// https://github.com/JonPSmith/EfCore.TestSupport/blob/master/TestSupport/EfHelpers/SqliteInMemory.cs
        /// </summary>
        private static DbContextOptionsBuilder<T> SetupConnectionAndBuilderOptions<T>()
            where T : DbContext
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:"
            };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open(); // See https://github.com/aspnet/EntityFramework/issues/6968

            // Create an in-memory context
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connection);
            return builder;
        }
    }
}
