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

    [Route("api/config")]
    public string[] Get()
    {
      var configPath = ConfigurationManager.AppSettings[CONFIG_PATH_SETTING];

      return Directory.GetFiles(configPath)
        .Select(file => MakeRelativePath(configPath, file))
        .ToArray();
    }

    [Route("api/config/{file}")]
    public Config Get(string file)
    {
      return ConfigManager.ReadFromFile(Path.Combine(ConfigurationManager.AppSettings[CONFIG_PATH_SETTING], file));
    }

    [Route("api/config")]
    public void Put([FromBody]Config config)
    {
      var id = Guid.NewGuid().ToString("N");
      ConfigManager.SaveFile(config, Path.Combine(ConfigurationManager.AppSettings[CONFIG_PATH_SETTING], id));
    }



    public static String MakeRelativePath(String fromPath, String toPath)
    {
      if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
      if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

      Uri fromUri = new Uri(fromPath);
      Uri toUri = new Uri(toPath);

      if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
      {
        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      }

      return relativePath;
    }

  }
}
