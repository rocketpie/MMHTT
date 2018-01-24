using MMHTT.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MMHTT.Domain.Managers
{
  internal class AgentManager
  {
    internal static Agent[] InitializeAgents(ILog log, Config config, ConnectionManager connectionManager, Supervisor supervisor, IRequestRenderer renderer)
    {
      List<Agent> agents = new List<Agent>();
      var defaultAgentBehaviour = config.AgentBehaviours.FirstOrDefault(a => a.Agent == "Default") ?? new AgentBehaviour();

      var agentNames = config.RequestDefinitions.SelectMany(r => r.Agents).Distinct();
      foreach (var agentName in agentNames)
      {
        var requests = config.RequestDefinitions
          .Where(request => request.Agents
            .Any(requestAgent => requestAgent == agentName)
          ).ToArray();

        var behaviour = config.AgentBehaviours
          .FirstOrDefault(a => a.Agent == agentName)
          ?? defaultAgentBehaviour;

        agents.Add(new Agent(log, agentName, behaviour, requests, connectionManager.GetNewConnection(), supervisor, renderer));
      }

      return agents.ToArray();
    }
  }
}