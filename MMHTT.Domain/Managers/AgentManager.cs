using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MMHTT.Domain.Managers
{
  internal class AgentManager
  {
    ILog _log;
    ConnectionManager _connectionManager;
    CancellationToken _token;
    Dictionary<string, Agent> _agents = new Dictionary<string, Agent>();

    internal AgentManager(ILog log, CancellationToken token, ConnectionManager connectionManager)
    {
      _log = log;
      _connectionManager = connectionManager;
      _token = token;
    }

    internal Agent[] GenerateAgents(Settings settings)
    {
      ThrowOnDuplicateAgent(settings.AgentBehaviour);

      foreach (var behaviour in settings.AgentBehaviour)
      {
        _agents.Add(behaviour.Agent, new Agent(behaviour.Agent, _log, _token, _connectionManager.GetNewConnection(), behaviour, null, null));
      }

      return _agents.Values.ToArray();
    }

    private static void ThrowOnDuplicateAgent(AgentBehaviour[] agentBehaviour)
    {
      var behaviour = agentBehaviour.GroupBy(a => a.Agent);
      var duplicateAgents = string.Join(", ", behaviour.Where(a => a.Count() > 2));
      if (!string.IsNullOrWhiteSpace(duplicateAgents))
      {
        throw new SettingsException("duplicate AgentBehaviuor for Agent(s): " + duplicateAgents);
      }
    }
  }
}
