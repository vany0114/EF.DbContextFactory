using EFCore.DbContextFactory.Examples.Data.Persistence;
using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EFCore.DbContextFactory.Extensions;

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
            
            services.AddDbContextFactory<OrderContext>(builder => builder
                .UseInMemoryDatabase("OrdersExample"));

            services.AddScoped<OrderRepositoryWithFactory, OrderRepositoryWithFactory>();
            services.AddTransient<OrderRepository, OrderRepository>();
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
