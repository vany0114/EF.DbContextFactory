using System;
using System.Data.Entity;
using EF.DbContextFactory.Unity.Helpers;
using Unity;
using Unity.Injection;

namespace EF.DbContextFactory.Unity.Extensions
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="container"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        public static void AddDbContextFactory<TDataContext>(this IUnityContainer container, string nameOrConnectionString = null)
            where TDataContext : DbContext
        {
            nameOrConnectionString = nameOrConnectionString ?? "DefaultConnection";
            container.RegisterType<Func<TDataContext>>(new InjectionFactory(factory =>
            {
                if (!string.IsNullOrEmpty(nameOrConnectionString) && ReflectionHelper.HasContructorWithConnectionString<TDataContext>())
                    return new Func<TDataContext>(() => (TDataContext)Activator.CreateInstance(typeof(TDataContext), nameOrConnectionString));

                return new Func<TDataContext>(() => (TDataContext) Activator.CreateInstance(typeof(TDataContext)));
            }));
        }
    }
}