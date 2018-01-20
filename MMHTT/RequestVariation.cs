namespace MMHTT
{
  /// <summary>
  /// Definition of a type of request
  /// </summary>
  public class RequestVariation
  {           
    /// <summary>
    /// Agent to use this variation. 
    /// </summary>
    public string Agent { get; set; }
    /// <summary>
    /// when multiple request variations are defined, this controls how often this variation is used relative to the others.
    /// n = (this.Weight / sum(Weight)) * 
    /// </summary>
    public int Weight { get; set; }

    public string Endpoint { get; set; }
    public string Template { get; set; }  
    public string[] DynamicContent { get; set; }

  }
}