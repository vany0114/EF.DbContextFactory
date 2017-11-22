using System;
using System.Data.Entity;
using EF.DbContextFactory.Ninject.Helpers;
using Ninject.Syntax;

namespace EF.DbContextFactory.Ninject.Extensions
{
    public static class NinjectExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="binding"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        public static void AddDbContextFactory<TDataContext>(this IBindingRoot binding, string nameOrConnectionString = null)
            where TDataContext : DbContext
        {
            nameOrConnectionString = nameOrConnectionString ?? "DefaultConnection";
            binding.Bind<Func<TDataContext>>().ToMethod(ctx =>
            {
                if (!string.IsNullOrEmpty(nameOrConnectionString) && ReflectionHelper.HasContructorWithConnectionString<TDataContext>())
                    return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), nameOrConnectionString);

                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext));
            });
        }
    }
}