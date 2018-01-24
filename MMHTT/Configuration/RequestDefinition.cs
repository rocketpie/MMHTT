namespace MMHTT.Configuration
{
  /// <summary>
  /// Main InformatEntryDefinition of a type of request
  /// </summary>
  public class RequestDefinition
  {
    /// <summary>
    /// Agent to use this variation. 
    /// </summary>
    public string[] Agents { get; set; }
    /// <summary>
    /// when multiple request variations are defined for a single agent, this controls how often this variation is used relative to the others.
    /// n = (this.Weight / sum(Weight)) * 
    /// </summary>
    public int Weight { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string TemplateName { get; set; }

    public KeyValue[] KeyValues { get; set; }

    /// <summary>
    /// Render helper function to return the first key value to match the given key.
    /// otherwise, an empty string.
    /// comparison ignores case.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string KeyValue(string key)
    {
      for (int i = 0; i < KeyValues.Length; i++)
      {
        if (string.Compare(KeyValues[i].Key, key, true) == 0) { return KeyValues[i].Value; }
      }
      return "";
    }

  }
}