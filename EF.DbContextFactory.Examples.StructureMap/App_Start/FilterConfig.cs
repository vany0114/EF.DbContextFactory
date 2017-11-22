using System.Web.Mvc;

namespace EF.DbContextFactory.Examples.StructureMap
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
