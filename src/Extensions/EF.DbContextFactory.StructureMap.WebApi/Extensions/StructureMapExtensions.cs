using System;
using System.Data.Entity;
using EF.DbContextFactory.StructureMap.WebApi.Helpers;
using StructureMap;

namespace EF.DbContextFactory.StructureMap.WebApi.Extensions
{
    public static class StructureMapExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="registry"></param>
        /// <param name="nameOrConnectionString">Name or connection string of the context. (Optional)</param>
        public static void AddDbContextFactory<TDataContext>(this IRegistry registry, string nameOrConnectionString = null)
            where TDataContext : DbContext
        {
            registry.For<Func<TDataContext>>().Use(
                DbContextFactory.Create<TDataContext>(nameOrConnectionString)
            );
        }

        internal class DbContextFactory
        {
            public static Func<TDataContext> Create<TDataContext>(string nameOrConnectionString = null) 
                where TDataContext : DbContext
            {
                nameOrConnectionString = nameOrConnectionString ?? "DefaultConnection";
                if (!string.IsNullOrEmpty(nameOrConnectionString) && ReflectionHelper.HasContructorWithConnectionString<TDataContext>())
                    return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), nameOrConnectionString);

                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext));
            }
        }
    }
}