using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MMHTT.Domain
{
  /// <summary>
  /// Agent coordination, cancellation.
  /// </summary>
  public class Supervisor
  {
    public event EventHandler<EventArgs> Started;
    /// <summary>
    /// Once this Token was signalled, Agents must Stop making new Requests
    /// </summary>
    public CancellationToken CancellationToken { get { return _cancellation.Token; } }

    /// <summary>
    /// while this property is true, too many requests are send per second. Agents should skip a request or wait until this signal is false again.
    /// 'Important' requests may still be sent.
    /// </summary>
    public bool ShouldSkipRequest { get { return _shouldSkipRequest; } }
    volatile bool _shouldSkipRequest;

    /// <summary>
    /// Agents must call this method when 
    /// </summary>
    public void SignalRequestSent()
    {
      Interlocked.Increment(ref _totalAgentRequests);
    }

    /// <summary>
    /// Signal Agents to start working 
    /// TODO: test restart behaviour
    /// </summary>
    public void Start()
    {
      if (Started == null)
      {
        _log.Error($"No Agent is listening...");
        return;
      }

      _stopwatch = Stopwatch.StartNew();
      Task.Run((Action)WatchLimits);
      Started.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// User interruption option
    /// </summary>
    public void Abort()
    {
      _cancellation.Cancel();
    }

    /// <summary>
    /// number of requests sent by all Agents
    /// </summary>
    long _totalAgentRequests;
    Stopwatch _stopwatch;

    ILog _log;
    long _maxTotalRequests;
    int _maxRequestPerSecond;
    TimeSpan _maxRuntime;
    CancellationTokenSource _cancellation = new CancellationTokenSource();

    public Supervisor(ILog log, TimeSpan maxRuntime, int maxTotalRequests, int maxRequestPerSecond)
    {
      _log = new ContextLog(log, nameof(Supervisor));
      _maxTotalRequests = maxTotalRequests;
      _maxRequestPerSecond = maxRequestPerSecond;
      _maxRuntime = maxRuntime;
    }

    void WatchLimits()
    {
      while (!_cancellation.Token.IsCancellationRequested)
      {
        if (_totalAgentRequests > _maxTotalRequests)
        {
          _log.Info($"cancelling: hit MaxTotalRequests limit");
          _cancellation.Cancel();
        }

        if (_stopwatch.Elapsed > _maxRuntime)
        {
          _log.Info($"cancelling: hit MaxRuntime limit");
          _cancellation.Cancel();
        }

        decimal elapsedSeconds = (decimal)_stopwatch.ElapsedMilliseconds / 1000;
        if (elapsedSeconds == 0) { elapsedSeconds = 1; }

        decimal currentRequestPerSecond = (decimal)_totalAgentRequests / elapsedSeconds;
        _shouldSkipRequest = currentRequestPerSecond > _maxRequestPerSecond;
        if (ShouldSkipRequest)
        {
          _log.Debug($"limiting MaxRequestPerSecond");
        }

        Thread.Sleep(100);
      }
    }

  }
}