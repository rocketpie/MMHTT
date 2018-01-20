using MMHTT.Domain.Managers;
using System;
using System.Threading;

namespace MMHTT.Domain
{
  public class TestManager
  {
    ILog _log;
    Settings _settings;
    ConnectionManager _connectionManager;
    CancellationTokenSource _cancellation;

    Agent[] _agents;

    private TestManager() { }

    public static TestManager Parse(Settings settings, ILog log = null)
    {
      var result = new TestManager()
      {
        _cancellation = new CancellationTokenSource(),
        _log = log ?? new NullLog(),
        _settings = settings
      };

      if (result._settings == null) { throw new ArgumentNullException("cannot work without settings"); }

      result._connectionManager = new ConnectionManager(result._log);

      var agentGenerator = new AgentManager(result._log, result._cancellation.Token, result._connectionManager);
      result._agents = agentGenerator.GenerateAgents(settings);

      return result;
    }

    public void Run()
    {
      try
      {
        foreach (var agent in _agents)
        {
          agent.Run();
        }
      }
      catch (Exception ex)
      {
        _log.Error("cannot Start Test(s): " + ex.Message, ex);
      }
    }

    public void Stop()
    {
      _cancellation.Cancel();
    }

  }
}