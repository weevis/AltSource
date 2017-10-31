using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AltSourceWebBanking
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["api_address"] = this.getApiAddress();
        }

        protected string getApiAddress()
        {
            System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);

            if (config.AppSettings.Settings.Count > 0)
            {
                var api_domain = config.AppSettings.Settings["API_DOMAIN"].ToString();
                var api_port = config.AppSettings.Settings["API_Port"].ToString();

                return api_domain + ":" + api_port;
            }
            return null;
        }
    }
}
