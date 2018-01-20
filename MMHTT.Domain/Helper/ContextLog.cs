using System;

namespace MMHTT.Domain
{
  internal class ContextLog : ILog
  {
    ILog _log;
    string _context;

    internal ContextLog(ILog log, string context)
    {
      _log = log;
      _context = context;
    }

    public void Error(string message, Exception ex = null)
    {
      _log.Error($"({_context}) {message}", ex);
    }
  }
}