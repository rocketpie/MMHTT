using System;
using System.Net.Http;

namespace MMHTT.Domain.Helper
{
  internal class Connection : IDisposable
  {
    string _id;
    ILog _log;
    object _clientLock = new object();
    HttpClient _client = new HttpClient();

    internal Connection(ILog log)
    {
      _id = Guid.NewGuid().ToString("N").Substring(5);
      _log = new ContextLog(log, $"con:{_id}");
    }

    internal delegate void UseClientDelegate(ILog log, HttpClient client);

    internal void UseClient(UseClientDelegate useClient)
    {
      lock (_clientLock)
      {
        try
        {
          useClient(_log, _client);
        }
        catch (Exception ex)
        {
          _log.Error("useclient failed", ex);
        }
      }
    }

    public void Dispose()
    {
      _client.Dispose();
    }

  }
}