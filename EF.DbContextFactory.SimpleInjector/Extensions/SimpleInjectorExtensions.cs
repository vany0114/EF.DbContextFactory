using System;
using System.Data.Entity;
using EF.DbContextFactory.SimpleInjector.Helpers;
using SimpleInjector;

namespace EF.DbContextFactory.SimpleInjector.Extensions
{
    public static class SimpleInjectorExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="container"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        public static void AddDbContextFactory<TDataContext>(this Container container, string nameOrConnectionString = null)
            where TDataContext : DbContext
        {
            nameOrConnectionString = nameOrConnectionString ?? "DefaultConnection";
            container.Register<Func<TDataContext>>(() =>
            {
                if (!string.IsNullOrEmpty(nameOrConnectionString) && ReflectionHelper.HasContructorWithConnectionString<TDataContext>())
                    return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), nameOrConnectionString);

                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext));
            }, Lifestyle.Scoped);
        }
    }
}