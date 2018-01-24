namespace MMHTT.Configuration
{
  public class AgentBehaviour
  {
    /// <summary>
    /// Special Name: 'Default'
    /// </summary>
    public string Agent { get; set; }

    /// <summary>
    /// behaviour control
    /// </summary>
    public int MaxRequestsPerSecond { get; set; }

    public KeyValue[] InitialSessionData { get; set; }
  }
}
