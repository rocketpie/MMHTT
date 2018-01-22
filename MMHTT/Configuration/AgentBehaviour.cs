namespace MMHTT.Configuration
{
  public class AgentBehaviour
  {
    public static AgentBehaviour Default = null;

    public static AgentBehaviour GetDefaultBehaviour() => Default ?? new AgentBehaviour()
    {
      MaxRequestsPerSecond = 1
    };

    /// <summary>
    /// Special Name: 'Default'
    /// </summary>
    public string Agent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxRequestsPerSecond { get; set; }
  }
}
