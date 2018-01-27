using System;
using System.Net.Http;
using System.Threading.Tasks;

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

    internal delegate Task UseClientDelegate(ILog log, HttpClient client);

    internal async Task UseClient(UseClientDelegate useClient)
    {
      try
      {
        await useClient(_log, _client);
      }
      catch (Exception ex)
      {
        _log.Error("useclient failed", ex);
      }
    }

    public void Dispose()
    {
      _client.Dispose();
    }

  }
}