using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(MMHTT.HttpApi.WebApiApplication))]
namespace MMHTT.HttpApi
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    public void Configuration(IAppBuilder app)
    {
      app.MapSignalR();
    }

    protected void Application_Start()
    {
      GlobalConfiguration.Configure(WebApiConfig.Register);
    }
  }
}