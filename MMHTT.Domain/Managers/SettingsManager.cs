using Newtonsoft.Json;

namespace MMHTT.Domain
{
  public class SettingsManager
  {
    public static Settings ReadFromFile(string path)
    {
      var content = System.IO.File.ReadAllText(path);
      Settings settings = JsonConvert.DeserializeObject<Settings>(content);

      return settings;
    }

    public static void SaveFile(Settings settings, string path)
    {
      var content = JsonConvert.SerializeObject(settings);
      System.IO.File.WriteAllText(path, content);
    }
  }
}