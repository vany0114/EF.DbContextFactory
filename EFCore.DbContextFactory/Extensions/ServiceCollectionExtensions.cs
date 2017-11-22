using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFCore.DbContextFactory.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        /// <param name="logger">The <see cref="ILoggerFactory" implementation./></param>
        public static void AddSqlServerDbContextFactory<TDataContext>(this IServiceCollection services, string nameOrConnectionString = null, ILoggerFactory logger = null)
            where TDataContext : DbContext
        {
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();
                nameOrConnectionString = configuration.GetConnectionString("DefaultConnection");
            }

            services.AddScoped<Func<TDataContext>>(ctx =>
            {
                var options = new DbContextOptionsBuilder<TDataContext>()
                    .UseSqlServer(nameOrConnectionString)
                    .UseLoggerFactory(logger)
                    .Options;

                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options);
            });
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">The DbContext options.</param>
        public static void AddDbContextFactory<TDataContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TDataContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TDataContext>(new DbContextOptions<TDataContext>());
            options.Invoke(builder);

            services.AddScoped<Func<TDataContext>>(ctx =>
            {
                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), builder.Options);
            });
        }
    }
}
