using MMHTT.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MMHTT.Domain.Managers
{
  internal class AgentManager
  {
    internal static Agent[] InitializeAgents(ILog log, Config settings, ConnectionManager connectionManager, Supervisor supervisor, IRequestRenderer renderer)
    {
      Dictionary<string, Agent> _agents = new Dictionary<string, Agent>();

      var agentRequestsGroups = settings.RequestDefinitions.GroupBy(v => v.Agent ?? nameof(AgentBehaviour.Default));
      foreach (var agentRequests in agentRequestsGroups)
      {
        var behaviour = settings.AgentBehaviours?.FirstOrDefault(b => b.Agent == agentRequests.Key) ?? AgentBehaviour.GetDefaultBehaviour();

        _agents.Add(agentRequests.Key, new Agent(log, agentRequests.Key, behaviour, agentRequests.ToArray(), connectionManager.GetNewConnection(), supervisor, renderer));
      }

      return _agents.Values.ToArray();
    }

  }
}