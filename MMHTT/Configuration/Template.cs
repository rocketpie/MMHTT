namespace MMHTT.Configuration
{
  public class Template
  {
    /// <summary>
    /// Template name to use for reference / debugging
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// if file is set, read from there
    /// </summary>
    public string File { get; set; }

    /// <summary>
    /// actual template
    /// </summary>
    public string TemplateString { get; set; }

  }
}