namespace MMHTT
{
  public class Settings
  {
    /// <summary>
    /// providing a file will override all RequestVaritions with the one's found in the file
    /// </summary>
    public string RequestVariationsFile { get; set; }
    public RequestVariation[] RequestVariations { get; set; }

    /// <summary>
    /// request templates (razor?) will be rendered with a requestVariation
    /// </summary>
    public Template[] Templates { get; set; }

    /// <summary>
    /// optional agent behaviour control
    /// </summary>
    public AgentBehaviour[] AgentBehaviour { get; set; }

    /// <summary>
    /// Stop Test afer reaching this number of Requests across all agents
    /// </summary>
    public int MaxTotalRequests { get; set; }

    /// <summary>
    /// IOC. type? assembly? ??? 
    /// </summary>
    public string Renderer { get; set; }
  }
}