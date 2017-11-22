using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.DbContextFactory.Examples.Data.Persistence;
using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using EFCore.DbContextFactory.Extensions;

namespace EFCore.DbContextFactory.Examples.WebApi
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

            var dbLogger = new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

            // *********************** normal way to inject a DbContext in .netcore ***********************
            services.AddDbContext<OrderContext>(builder =>
                builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // ***********AddSqlServerDbContextFactory to use sql server directly (the easiest way)********
            services.AddSqlServerDbContextFactory<OrderContext>();

            // ********************************************************************************************
            // Other ways to add the DbContext factory. (sending the options builder, you can build your options as you need.)

            // ************************************sql server**********************************************
            //services.AddDbContextFactory<OrderContext>(builder => builder
            //    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            //    .UseLoggerFactory(dbLogger));

            // ************************************sqlite**************************************************
            //services.AddDbContextFactory<OrderContext>(builder => builder
            //    .UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
            //    .UseLoggerFactory(dbLogger));

            // ************************************in memory***********************************************
            //services.AddDbContextFactory<OrderContext>(builder => builder
            //    .UseInMemoryDatabase("OrdersExample")
            //    .UseLoggerFactory(dbLogger));

            services.AddScoped<OrderRepositoryWithFactory, OrderRepositoryWithFactory>();
            services.AddScoped<OrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
