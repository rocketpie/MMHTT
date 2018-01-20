namespace MMHTT
{
  public class AgentBehaviour
  {
    public static AgentBehaviour Default = null;

    public static AgentBehaviour GetDefaultBehaviour() => Default ?? new AgentBehaviour()
    {
      MaxRequestsPerSecond = 1
    };

    /// <summary>
    /// Special Name 'Default' will override the 
    /// </summary>
    public string Agent { get; set; }

    public int MaxRequestsPerSecond { get; set; }
  }
}
