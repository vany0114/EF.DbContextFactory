using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EFCore.DbContextFactory.Extensions
{
    /// <summary>
    /// Extensions to add AddDbContextFactory
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        /// <param name="logger">The <see cref="ILoggerFactory"/>implementation.</param>
        public static void AddSqlServerDbContextFactory<TDataContext>(this IServiceCollection services, string nameOrConnectionString = null, ILoggerFactory logger = null)
            where TDataContext : DbContext
        {
            services.AddSqlServerDbContextFactory<TDataContext, TDataContext>(nameOrConnectionString, logger);
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TContextService"/>'s factory.
        /// </summary>
        /// <typeparam name="TContextService">The DbContext service type.</typeparam>
        /// <typeparam name="TContextImplementation">The DbContent implementation type.</typeparam>
        /// <param name="services"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        /// <param name="logger">The <see cref="ILoggerFactory"/>implementation.</param>
        public static void AddSqlServerDbContextFactory<TContextService, TContextImplementation>(this IServiceCollection services, string nameOrConnectionString = null, ILoggerFactory logger = null)
            where TContextService : class
            where TContextImplementation : DbContext, TContextService
        {
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();
                nameOrConnectionString = configuration.GetConnectionString("DefaultConnection");
            }

            AddDbContextFactory<TContextService, TContextImplementation>(services, (provider, builder) =>
                builder.UseSqlServer(nameOrConnectionString)
                    .UseLoggerFactory(logger)
            );
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">The DbContext options.</param>
        public static void AddDbContextFactory<TDataContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TDataContext : DbContext
            => AddDbContextFactory<TDataContext, TDataContext>(services, (provider, builder) => options.Invoke(builder));

        /// <summary>
        /// Configures the resolution of <typeparamref name="TContextService"/>'s factory.
        /// </summary>
        /// <typeparam name="TContextService">The DbContext service type.</typeparam>
        /// <typeparam name="TContextImplementation">The DbContext implementation type.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">The DbContext options.</param>
        public static void AddDbContextFactory<TContextService, TContextImplementation>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TContextService : class
            where TContextImplementation : DbContext, TContextService
            => AddDbContextFactory<TContextService, TContextImplementation>(services, (provider, builder) => options.Invoke(builder));

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Service provider and DbContext options.</param>
        public static void AddDbContextFactory<TDataContext>(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TDataContext : DbContext
        {
            services.AddDbContextFactory<TDataContext, TDataContext>(optionsAction);
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TContextService"/>'s factory.
        /// </summary>
        /// <typeparam name="TContextService">The DbContext service type.</typeparam>
        /// <typeparam name="TContextImplementation">The DbContext implementation type.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Service provider and DbContext options.</param>
        public static void AddDbContextFactory<TContextService, TContextImplementation>(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TContextService : class
            where TContextImplementation : DbContext, TContextService
        {
            AddCoreServices<TContextService, TContextImplementation>(services, optionsAction, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<DbContextOptions<TContextImplementation>>();

            services.AddScoped<Func<TContextService>>(ctx => () => (TContextService)Activator.CreateInstance(typeof(TContextImplementation), options));
        }

        private static void AddCoreServices<TContextService, TContextImplementation>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime)
            where TContextService : class
            where TContextImplementation : DbContext, TContextService
        {
            serviceCollection
                .AddMemoryCache()
                .AddLogging();

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContextImplementation>),
                    p => DbContextOptionsFactory<TContextImplementation>(p, optionsAction),
                    optionsLifetime));

            serviceCollection.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContextImplementation>>(),
                    optionsLifetime));
        }

        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>(
                new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);
            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}