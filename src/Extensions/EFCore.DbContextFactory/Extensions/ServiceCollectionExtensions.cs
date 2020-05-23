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
        /// <param name="logger">The <see cref="ILoggerFactory"/>implementation. (Optional)</param>
        /// <param name="optionsBuilderFactory">A factory for the DbContext Options Builder. (Optional)</param>
        public static IServiceCollection AddSqlServerDbContextFactory<TDataContext>(this IServiceCollection services,
            string nameOrConnectionString = null,
            ILoggerFactory logger = null,
            Func<DbContextOptionsBuilder<TDataContext>> optionsBuilderFactory = null)
            where TDataContext : DbContext
        {
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();
                nameOrConnectionString = configuration.GetConnectionString("DefaultConnection");
            }

            return AddDbContextFactory<TDataContext>(
                services,
                (provider, builder) => builder.UseSqlServer(nameOrConnectionString).UseLoggerFactory(logger),
                optionsBuilderFactory
            );
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">Returns the DbContext options.</param>
        public static IServiceCollection AddDbContextFactory<TDataContext>(
            this IServiceCollection services,
            Func<DbContextOptions> options)
            where TDataContext : DbContext
            => AddDbContextFactory<TDataContext>(
                services,
                (provider) => options.Invoke());

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsFunc">Service provider and DbContext options.</param>
        public static IServiceCollection AddDbContextFactory<TDataContext>(
            this IServiceCollection services,
            Func<IServiceProvider, DbContextOptions> optionsFunc)
            where TDataContext : DbContext
        {
            AddCoreServices<TDataContext>(services, optionsFunc, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<DbContextOptions<TDataContext>>();

            return services.AddScoped<Func<TDataContext>>(ctx => () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options));
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Processes the DbContext options.</param>
        /// <param name="optionsBuilderFactory">A factory for the DbContext Options Builder. (Optional)</param>
        public static IServiceCollection AddDbContextFactory<TDataContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TDataContext>> optionsBuilderFactory = null)
            where TDataContext : DbContext
            => AddDbContextFactory<TDataContext>(services, (provider, builder) => optionsAction.Invoke(builder), optionsBuilderFactory);

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Service provider and DbContext options.</param>
        /// <param name="optionsBuilderFactory">A factory for the DbContext Options Builder. (Optional)</param>
        public static IServiceCollection AddDbContextFactory<TDataContext>(this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TDataContext>> optionsBuilderFactory = null)
            where TDataContext : DbContext
        {
            AddCoreServices<TDataContext>(services, optionsAction, optionsBuilderFactory, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<DbContextOptions<TDataContext>>();

            return services.AddScoped<Func<TDataContext>>(ctx => () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options));
        }

        private static void AddCoreServices<TContextImplementation>(
            IServiceCollection services,
            Func<IServiceProvider, DbContextOptions> optionsFunc,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : DbContext
        {
            services
                .AddMemoryCache()
                .AddLogging();

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContextImplementation>),
                    p => optionsFunc(p),
                    optionsLifetime));

            services.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContextImplementation>>(),
                    optionsLifetime));
        }

        private static void AddCoreServices<TContextImplementation>(
            IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TContextImplementation>> dbContextBuilderFactory,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : DbContext
        {
            AddCoreServices<TContextImplementation>(
                services,
                p => DbContextOptionsFactory(p, optionsAction, dbContextBuilderFactory),
                optionsLifetime);
        }

        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TContext>> optionsBuilderFactory)
            where TContext : DbContext
        {
            DbContextOptionsBuilder<TContext> builder = optionsBuilderFactory?.Invoke()
                ?? new DbContextOptionsBuilder<TContext>(
                    new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);
            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}