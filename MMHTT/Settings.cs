namespace MMHTT
{
  public class Settings
  {
    /// <summary>
    /// number of simulated 'users' that concurrently access the tested resource
    /// </summary>
    public int AgentCount { get; set; }

    /// <summary>
    /// same as AgentCount ?
    /// </summary>
    public int MaxConcurrentConnections { get; set; }

    /// <summary>
    /// Stop Test afer reaching this number of Requests across all agents
    /// </summary>
    public int MaxTotalRequests { get; set; }

    public AgentBehaviour[] AgentBehaviour { get; set; }

    public RequestVariation[] RequestVariations { get; set; }

    public DynamicContent[] DynamicContent { get; set; }
  }
}