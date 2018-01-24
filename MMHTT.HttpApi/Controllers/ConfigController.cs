using MMHTT.Configuration;
using MMHTT.Domain;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace MMHTT.HttpApi.Controllers
{
  public class ConfigController : ApiController
  {
    const string CONFIG_PATH_SETTING = "ConfigPath";

    public Config[] Get()
    {
      return Directory.GetFiles(ConfigurationManager.AppSettings[CONFIG_PATH_SETTING])
        .Select(ConfigManager.ReadFromFile)
        .ToArray();
    }

    public void Put([FromBody]Config config)
    {
      var id = Guid.NewGuid().ToString("N");
      ConfigManager.SaveFile(config, Path.Combine(ConfigurationManager.AppSettings[CONFIG_PATH_SETTING], id));
    }
  }
}
