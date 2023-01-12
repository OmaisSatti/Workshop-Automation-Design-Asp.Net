using System.Web;
using System.Web.Mvc;

namespace Vehical_Workshop_Automation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
