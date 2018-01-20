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
    ThresholdDispatcher<RequestVariation> _variationDispatcher;

    internal Agent(
      string id,
      ILog log,
      CancellationToken token,
      Connection connection,
      AgentBehaviour behaviour,
      RequestVariation[] variations,
      IRequestRenderer renderer)
    {
      _id = id;
      _log = new ContextLog(log, $"agent:{id}");
      _token = token;
      _connection = connection;
      _behaviour = behaviour;
      _variationDispatcher = new ThresholdDispatcher<RequestVariation>(variations, v => v.Weight);
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

      var request = _renderer.Render(_variationDispatcher.Dispatch());

      _connection.UseClient((connectionLog, client) =>
      {
        var responseTask = client.SendAsync(request);
      });
    }

  }
}
