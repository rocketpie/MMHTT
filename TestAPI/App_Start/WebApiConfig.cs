using System.Web.Http;

namespace TestAPI
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web-API-Konfiguration und -Dienste

      // Web-API-Routen
      config.MapHttpAttributeRoutes();

    }
  }
}