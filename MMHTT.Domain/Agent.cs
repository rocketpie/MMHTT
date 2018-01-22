using MMHTT.Configuration;
using MMHTT.Domain.Helper;
using System.Threading;
using System.Timers;

namespace MMHTT.Domain
{
  internal class Agent
  {
    string _id;
    ILog _log;
    CancellationToken _token;
    Connection _connection;
    AgentBehaviour _behaviour;
    IRequestRenderer _renderer;

    System.Timers.Timer _signal;
    ThresholdDispatcher<RequestDefinition> _requestDispatcher;

    internal Agent(
      string id,
      ILog log,
      CancellationToken token,
      Connection connection,
      AgentBehaviour behaviour,
      RequestDefinition[] requests,
      IRequestRenderer renderer)
    {
      _id = id;
      _log = new ContextLog(log, $"agent:{id}");
      _token = token;
      _connection = connection;
      _behaviour = behaviour;
      _requestDispatcher = new ThresholdDispatcher<RequestDefinition>(_log, requests, v => v.Weight);
      _renderer = renderer;

      var interval = (1000 / behaviour.MaxRequestsPerSecond);
      _signal = new System.Timers.Timer(interval);

      _token.Register(() => { _signal.Stop(); });
      _signal.Elapsed += _signal_Elapsed;
    }

    internal void Run()
    {
      _signal.Start();
    }

    private void _signal_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (_token.IsCancellationRequested) { return; }

      var request = _renderer.Render(_requestDispatcher.Dispatch());

      _connection.UseClient((connectionLog, client) =>
      {
        var responseTask = client.SendAsync(request);
      });
    }

  }
}
