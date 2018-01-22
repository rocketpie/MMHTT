using MMHTT.Configuration;
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

    internal Agent[] GenerateAgents(Config settings)
    {
      var agentVariations = settings.RequestDefinitions.GroupBy(v => v.Agent);
      foreach (var agent in agentVariations)
      {
        var behaviour = settings.AgentBehaviours?.FirstOrDefault(b => b.Agent == agent.Key) ?? AgentBehaviour.GetDefaultBehaviour();
        _agents.Add(agent.Key ?? "", new Agent(agent.Key, _log, _token, _connectionManager.GetNewConnection(), behaviour, agent.ToArray(), null));
      }

      return _agents.Values.ToArray();
    }



  }
}