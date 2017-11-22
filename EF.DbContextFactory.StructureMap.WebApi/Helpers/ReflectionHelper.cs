using System.Linq;

namespace EF.DbContextFactory.StructureMap.WebApi.Helpers
{
    public class ReflectionHelper
    {
        public static bool HasContructorWithConnectionString<T>()
        {
            var ctors = typeof(T).GetConstructors();

            return ctors
                .Select(ctor =>
                    ctor.GetParameters())
                        .Any(parameters => parameters.Any(x => x.ParameterType == typeof(string)));
        }
    }
}