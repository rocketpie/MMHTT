using MMHTT.Configuration;
using MMHTT.Domain.Helper;
using System;
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

    System.Timers.Timer _signal;


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

      var maxRequestsPerSecond = behaviour.MaxRequestsPerSecond;
      if (maxRequestsPerSecond < 1) { maxRequestsPerSecond = 1000; }
      var interval = (1000 / maxRequestsPerSecond);
      _signal = new Timer(interval);

      _supervisor.Started += _supervisor_Started;
      _supervisor.CancellationToken.Register(Abort);
      _signal.Elapsed += _signal_Elapsed;
    }

    void _supervisor_Started(object sender, EventArgs e)
    {
      _signal.Start();
    }

    void Abort()
    {
      _signal.Stop();
    }

    private void _signal_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (_supervisor.CancellationToken.IsCancellationRequested)
      {
        Abort();
        return;
      }

      if (_supervisor.ShouldSkipRequest) { return; }

      var requestBase = _renderer.Render(_requestDispatcher.Dispatch());

      if (requestBase.Method == null)
      {
        _log.Error($"cannot send request: HTTP Method not set");
        return;
      }

      // also validate method content?
      Uri endpoint;
      if (!Uri.TryCreate(requestBase.Model.Endpoint, UriKind.Absolute, out endpoint))
      {
        _log.Error($"cannot send request: Endpoint is not a valid Uri");
        return;
      }

      var request = new HttpRequestMessage(new HttpMethod(requestBase.Method), endpoint);
      if (!string.IsNullOrEmpty(requestBase.Result))
      {
        request.Content = new StringContent(requestBase.Result);
      }

      if (requestBase.Headers.AllKeys.Any())
      {
        string currentHeaderName = "";
        try
        {
          foreach (var header in requestBase.Headers.AllKeys)
          {
            currentHeaderName = header;
            request.Headers.Add(header, requestBase.Headers[header]);
          }
        }
        catch (Exception ex)
        {
          _log.Error($"cannot set Header {{ \"Name\":\"${currentHeaderName}\", \"Value\":\"{requestBase.Headers?[currentHeaderName]}\" }}: {ex.Message}", ex);
          return;
        }
      }

      _connection.UseClient((connectionLog, client) =>
      {
        var responseTask = client.SendAsync(request);
        _supervisor.SignalRequestSent();

        try
        {
          using (HttpResponseMessage response = responseTask.Result)
          {
            using (HttpContent content = response.Content)
            {
              var responseText = content.ReadAsStringAsync().Result;
            }
          }
        }
        catch (Exception ex)
        {
          _log.Error($"021d64f8 Error: {ex.Message}", ex);
        }
      });
    }
  }
}
