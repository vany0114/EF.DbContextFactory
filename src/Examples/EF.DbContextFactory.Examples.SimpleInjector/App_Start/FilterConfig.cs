using System.Web;
using System.Web.Mvc;

namespace EF.DbContextFactory.Examples.SimpleInjector
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
