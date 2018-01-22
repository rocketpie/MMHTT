using MMHTT.Configuration;
using MMHTT.Domain.Helper;
using System;
using System.Linq;
using System.Net.Http;
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
      if (_token.IsCancellationRequested)
      {
        _signal.Stop();
        return;
      }

      var requestBase = _renderer.Render(_requestDispatcher.Dispatch());

      if (requestBase.Method == null)
      {
        _log.Error($"cannot send request: HTTP Method not set");
        return;
      }

      // also validate method content?

      if (!Uri.TryCreate(requestBase.Model.Endpoint, UriKind.Absolute, out Uri endpoint))
      {
        _log.Error($"cannot send request: Endpoint is not a valid Uri");
        return;
      }

      var request = new HttpRequestMessage(new HttpMethod(requestBase.Method), endpoint);
      request.Content = new StringContent(requestBase.Result);

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
        try
        {
          using (HttpResponseMessage response = client.SendAsync(request).Result)
          {
            using (HttpContent content = response.Content)
            {
              var responseText = content.ReadAsStringAsync().Result;
            }
          }
        }
        catch (Exception ex)
        {
          _log.Error($"http response error: {ex.Message}", ex);
        }
      });
    }
  }
}
