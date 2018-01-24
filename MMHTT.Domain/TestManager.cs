using MMHTT.Configuration;
using MMHTT.Domain.Managers;
using System;

namespace MMHTT.Domain
{
  public class TestManager
  {
    ILog _log;
    Config _config;
    ConnectionManager _connectionManager;

    Agent[] _agents;
    private Supervisor _supervisor;

    private TestManager() { }

    public static TestManager Initialize(Config config, IRequestRenderer renderer, ILog log = null)
    {
      ConfigManager.LoadAndTest(config);

      var result = new TestManager()
      {
        _log = log ?? new ConsoleLog(),
        _config = config
      };

      renderer.Initialize(result._config);

      result._connectionManager = new ConnectionManager(result._log);
      result._supervisor = new Supervisor(result._log, TimeSpan.FromSeconds(config.MaxTestRuntimeSeconds), config.MaxTotalRequests, config.MaxRequestsPerSecond);

      result._agents = AgentManager.InitializeAgents(result._log, config, result._connectionManager, result._supervisor, renderer);

      return result;
    }

    public void Start()
    {
      try
      {
        _supervisor.Start();
      }
      catch (Exception ex)
      {
        _log.Error("cannot Start Test(s): " + ex.Message, ex);
      }
    }

    public void Abort()
    {
      try
      {
        _supervisor.Abort();
      }
      catch (Exception ex)
      {
        _log.Error("cannot Abort Test(s): " + ex.Message, ex);
      }
    }

  }
}