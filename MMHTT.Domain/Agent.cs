using MMHTT.Configuration;
using MMHTT.Domain.Helper;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Timers;

namespace MMHTT.Domain
{
  internal class Agent
  {
    ILog _log;
    string _id;
    AgentBehaviour _behaviour;
    ThresholdDispatcher<RequestDefinition> _requestDispatcher;
    Connection _connection;
    Supervisor _supervisor;
    IRequestRenderer _renderer;

    /// <summary>
    /// Agent session data
    /// </summary>
    NameValueCollection _session;

    int _totalRequests = 0;
    /// <summary>
    /// Started when the Agent starts
    /// </summary>
    Stopwatch _stopwatch;
    Timer _signal;


    internal Agent(
      ILog log,
      string id,
      AgentBehaviour behaviour,
      RequestDefinition[] requests,
      Connection connection,
      Supervisor supervisor,
      IRequestRenderer renderer)
    {
      _log = new ContextLog(log, $"agent:{id}");
      _id = id;
      _behaviour = behaviour;
      _requestDispatcher = new ThresholdDispatcher<RequestDefinition>(_log, requests, v => v.Weight);
      _connection = connection;
      _supervisor = supervisor;
      _renderer = renderer;

      _session = new NameValueCollection();
      foreach (var item in _behaviour.InitialSessionData)
      {
        _session[item.Key] = item.Value;
      }

      var maxRequestsPerSecond = behaviour.MaxRequestsPerSecond;
      if (maxRequestsPerSecond < 1) { maxRequestsPerSecond = 10; }
      var interval = (1000 / maxRequestsPerSecond);
      if (interval < 1) { interval = 1; }
      _signal = new Timer(interval);

      _supervisor.Started += _supervisor_Started;
      _supervisor.CancellationToken.Register(Abort);
      _signal.Elapsed += _signal_Elapsed;
    }

    void _supervisor_Started(object sender, EventArgs e)
    {
      _stopwatch = Stopwatch.StartNew();
      _signal.Start();
    }

    void Abort()
    {
      _signal.Stop();
    }

    private void _signal_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (!ContinueRequest()) { return; }

      var requestBase = _renderer.Render(_requestDispatcher.Dispatch(), _session);

      HttpRequestMessage request;
      if (!TryCreateHttpRequest(requestBase, out request)) { return; }

      HttpResponseMessage response = null;
      _connection.UseClient((connectionLog, client) =>
      {
        var responseTask = client.SendAsync(request);
        _supervisor.SignalRequestSent();
        _totalRequests++;

        try
        {
          using (response = responseTask.Result)
          {
            using (HttpContent content = response.Content)
            {
              var responseText = content.ReadAsStringAsync().Result;
            }
          }
        }
        catch (HttpRequestException hex)
        {

          _log.Warn($"6f8a054f Error: {hex.ToString()}");
        }
        catch (Exception ex)
        {
          _log.Error($"021d64f8 Error: {ex.Message}", ex);
        }
      });

      if (requestBase.OnResponse != null)
      {
        try
        {
          requestBase.OnResponse(requestBase, response, _session);
        }
        catch (Exception ex)
        {
          _log.Warn($"{nameof(HttpRequestBase.OnResponse)} exception: {ex.ToString()}");
        }
      }

    }

    private bool TryCreateHttpRequest(HttpRequestBase requestBase, out HttpRequestMessage request)
    {
      request = null;

      if (requestBase == null)
      {
        _log.Error($"cannot send request: RequestRenderer returned null");
        return false;
      }

      // also validate method content?
      if (requestBase.Method == null)
      {
        _log.Error($"cannot send request: 'Method' not set");
        return false;
      }

      Uri endpoint;
      if (!Uri.TryCreate(requestBase.Url, UriKind.Absolute, out endpoint))
      {
        _log.Error($"cannot send request: 'Url' not set to a valid Uri");
        return false;
      }

      try
      {
        request = new HttpRequestMessage(new HttpMethod(requestBase.Method), endpoint);
        if (!string.IsNullOrEmpty(requestBase.RequestContent))
        {
          request.Content = new StringContent(requestBase.RequestContent);
        }

        if (requestBase.Headers.AllKeys.Any())
        {
          foreach (var header in requestBase.Headers.AllKeys)
          {
            request.Headers.Add(header, requestBase.Headers[header]);
          }
        }
      }
      catch (Exception ex)
      {
        _log.Error($"cannot create HttpRequestMessage: {ex.Message}", ex);
        return false;
      }

      return true;
    }

    private bool ContinueRequest()
    {
      /// aborted?
      if (_supervisor.CancellationToken.IsCancellationRequested)
      {
        Abort();
        return false;
      }

      /// supervisor: too many requests?
      return !_supervisor.ShouldSkipRequest;
    }

  }
}
