using Bot.Worker;
using System.Web.Http;

namespace CorporatePoolBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            HandlerInitializer.CreateAllHandlers();
        }
    }
}
