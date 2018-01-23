namespace MMHTT.Configuration
{
  /// <summary>
  /// Config -> RequestDefinition -> Template, KeyValues, AgentBehaviour
  /// </summary>
  public class Config
  {
    /// <summary>
    /// providing a file will override all RequestVaritions with the one's found in the file
    /// </summary>
    public string RequestDefinitionsFile { get; set; }
    public RequestDefinition[] RequestDefinitions { get; set; }

    /// <summary>
    /// request template files (razor) will be rendered with a requestVariation
    /// </summary>
    public Template[] Templates { get; set; }

    /// <summary>
    /// optional agent behaviour control
    /// </summary>
    public AgentBehaviour[] AgentBehaviours { get; set; }

    /// <summary>
    /// Stop Test afer reaching this number of Requests across all agents
    /// </summary>
    public int MaxTotalRequests { get; set; }
    /// <summary>
    /// Stop Test after this number of seconds
    /// </summary>
    public int MaxTestRuntimeSeconds { get; set; }
    /// <summary>
    /// limit average test frequency 
    /// </summary>
    public int MaxRequestsPerSecond { get; set; }
  }
}