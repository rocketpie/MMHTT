using MMHTT.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace MMHTT.Domain
{
  public static class ConfigManager
  {
    public static Config ReadFromFile(string path)
    {
      var content = File.ReadAllText(path);
      Config config = JsonConvert.DeserializeObject<Config>(content);

      return config;
    }

    public static void SaveFile(Config config, string path)
    {
      var content = JsonConvert.SerializeObject(config, Formatting.Indented);
      File.WriteAllText(path, content);
    }

    public static string ReadTemplateStringFromFile(string path)
    {
      return File.ReadAllText(path);
    }

    public static RequestDefinition[] ReadRequestDefinitionsFromFile(string path)
    {
      var content = File.ReadAllText(path);
      Config config = JsonConvert.DeserializeObject<Config>(content);

      return config.RequestDefinitions;
    }

    /// <summary>
    /// Settings sanity check. Throws on insane input.
    /// (Also Reads all referenced settings files) 
    /// </summary>
    /// <param name="config">Config to read, load and check for errors</param>
    public static void LoadAndTest(Config config)
    {
      if (config == null) { throw new SettingsException($"{nameof(Config)} object must not be null"); }

      if (config.AgentBehaviours != null)
      {
        // agent behaviour must be unique
        var behaviour = config.AgentBehaviours.GroupBy(a => a.Agent);
        var duplicateAgents = string.Join(", ", behaviour.Where(a => a.Count() > 2));
        if (!string.IsNullOrWhiteSpace(duplicateAgents))
        {
          throw new SettingsException($"must not provide more than one '{nameof(Config.AgentBehaviours)}' for any '{nameof(Agent)}'. (affected '{nameof(Agent)}'(s): " + duplicateAgents + ")");
        }
      }

      if (!string.IsNullOrWhiteSpace(config.RequestDefinitionsFile))
      {
        try { config.RequestDefinitions = ReadRequestDefinitionsFromFile(config.RequestDefinitionsFile); }
        catch (Exception ex) { throw new SettingsException($"cannot read '{nameof(Config.RequestDefinitionsFile)}: {ex.Message}", ex); }
      }

      // must find at least one request variation
      if (config.RequestDefinitions == null || config.RequestDefinitions.Length < 1)
      {
        throw new SettingsException($"must provide at least one '{nameof(Config.RequestDefinitions)}'");
      }

      // must find at least one template
      if (config.Templates == null || config.Templates.Length < 1)
      {
        throw new SettingsException($"must provide at least one '{nameof(Config.Templates)}'");
      }

      var templates = config.Templates.GroupBy(a => a.Name);
      var duplicateTemplateNames = string.Join(", ", templates.Where(a => a.Count() > 2));
      if (!string.IsNullOrWhiteSpace(duplicateTemplateNames))
      {
        throw new SettingsException($"must not provide more than one '{nameof(Config.Templates)}' with the same name. (affected '{nameof(Template)}'(s): " + duplicateTemplateNames + ")");
      }

      foreach (var template in config.Templates)
      {
        if (!string.IsNullOrWhiteSpace(template.File))
        {
          try { template.TemplateString = ReadTemplateStringFromFile(template.File); }
          catch (Exception ex) { throw new SettingsException($"cannot read {nameof(Template)} '{template.Name}' {nameof(Template.File)}: {ex.Message}", ex); }
        }

        if (template.Name == null) { throw new SettingsException($"must provide '{nameof(Template.Name)}' for each '{nameof(Config.Templates)}'"); }
        if (template.TemplateString == null) { throw new SettingsException($"must provide '{nameof(Template.TemplateString)}' for each '{nameof(Config.Templates)}'"); }
      }

      foreach (var request in config.RequestDefinitions)
      {
        if (request.Endpoint == null) { throw new SettingsException($"must provide '{nameof(RequestDefinition.Endpoint)}' for each '{nameof(Config.RequestDefinitions)}'"); }
        if (request.TemplateName == null) { throw new SettingsException($"must provide '{nameof(RequestDefinition.TemplateName)}' for each '{nameof(Config.RequestDefinitions)}'"); }
        if (!config.Templates.Any(t => t.Name == request.TemplateName)) { throw new SettingsException($"{nameof(RequestDefinition)}: cannot find '{nameof(Template)}' with '{nameof(Template.Name)}' '{request.TemplateName}'"); }

      }
    }

  }
}