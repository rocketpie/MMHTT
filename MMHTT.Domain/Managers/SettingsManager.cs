using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;

namespace MMHTT.Domain
{
  public static class SettingsManager
  {
    public static Settings ReadFromFile(string path)
    {
      var content = File.ReadAllText(path);
      Settings settings = JsonConvert.DeserializeObject<Settings>(content);

      return settings;
    }

    public static void SaveFile(Settings settings, string path)
    {
      var content = JsonConvert.SerializeObject(settings, Formatting.Indented);
      File.WriteAllText(path, content);
    }

    public static string ReadTemplateStringFromFile(string path)
    {
      return File.ReadAllText(path);
    }

    public static RequestVariation[] ReadRequestVariationsFromFile(string path)
    {
      var content = File.ReadAllText(path);
      Settings settings = JsonConvert.DeserializeObject<Settings>(content);

      return settings.RequestVariations;
    }

    /// <summary>
    /// Settings sanity check. Throws on insane input.
    /// (Also Reads all referenced settings files) 
    /// </summary>
    /// <param name="settings"></param>
    public static void LoadAndTest(Settings settings)
    {
      if (settings == null) { throw new SettingsException($"{nameof(Settings)} object must not be null"); }

      // agent behaviour must be unique
      if (settings.AgentBehaviour != null)
      {
        var behaviour = settings.AgentBehaviour.GroupBy(a => a.Agent);
        var duplicateAgents = string.Join(", ", behaviour.Where(a => a.Count() > 2));
        if (!string.IsNullOrWhiteSpace(duplicateAgents))
        {
          throw new SettingsException($"must not provide more than one '{nameof(Settings.AgentBehaviour)}' for any '{nameof(Agent)}'. (affected '{nameof(Agent)}'(s): " + duplicateAgents + ")");
        }
      }

      if (!string.IsNullOrWhiteSpace(settings.RequestVariationsFile))
      {
        try { settings.RequestVariations = ReadRequestVariationsFromFile(settings.RequestVariationsFile); }
        catch (Exception ex) { throw new SettingsException($"cannot read '{nameof(Settings.RequestVariationsFile)}: {ex.Message}", ex); }
      }

      // must find at least one request variation
      if (settings.RequestVariations == null || settings.RequestVariations.Length < 1)
      {
        throw new SettingsException($"must provide at least one '{nameof(Settings.RequestVariations)}'");
      }

      // must find at least one template
      if (settings.Templates == null || settings.Templates.Length < 1)
      {
        throw new SettingsException($"must provide at least one '{nameof(Settings.Templates)}'");
      }

      foreach (var template in settings.Templates)
      {
        if (!string.IsNullOrWhiteSpace(template.File))
        {
          try { template.TemplateString = ReadTemplateStringFromFile(template.File); }
          catch (Exception ex) { throw new SettingsException($"cannot read {nameof(Template)} '{template.Name}' {nameof(Template.File)}: {ex.Message}", ex); }
        }

        if (template.Name == null) { throw new SettingsException($"must provide '{nameof(Template.Name)}' for each '{nameof(Settings.Templates)}'"); }
        if (template.TemplateString == null) { throw new SettingsException($"must provide '{nameof(Template.TemplateString)}' for each '{nameof(Settings.Templates)}'"); }
      }

      foreach (var variation in settings.RequestVariations)
      {
        if (variation.Endpoint == null) { throw new SettingsException($"must provide '{nameof(RequestVariation.Endpoint)}' for each '{nameof(Settings.RequestVariations)}'"); }
        if (variation.TemplateName == null) { throw new SettingsException($"must provide '{nameof(RequestVariation.TemplateName)}' for each '{nameof(Settings.RequestVariations)}'"); }
        if (!settings.Templates.Any(t => t.Name == variation.TemplateName)) { throw new SettingsException($"{nameof(RequestVariation)}: cannot find '{nameof(Template)}' with '{nameof(Template.Name)}' '{variation.TemplateName}'"); }

      }
    }

  }
}