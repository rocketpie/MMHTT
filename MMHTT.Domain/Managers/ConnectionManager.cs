using MMHTT.Domain.Helper;
using System.Collections.Generic;

namespace MMHTT.Domain.Managers
{
  internal class ConnectionManager
  {                  
    List<Connection> _connections = new List<Connection>();
    private ILog _log;

    public ConnectionManager(ILog _log)
    {
      this._log = _log;
    }

    internal Connection GetNewConnection()
    {
      var connection = new Connection(_log);
      _connections.Add(connection);
      return connection;
    }

    ~ConnectionManager()
    {
      if (_connections != null)
      {
        foreach (var connection in _connections)
        {
          try
          {
            connection.Dispose();
          }
          catch { }
        }
      }
    }

  }
}