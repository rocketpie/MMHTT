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

    public void Debug(string message) => _log.Debug($"({_context}) {message}");
    public void Info(string message) => _log.Info($"({_context}) {message}");
    public void Warn(string message) => _log.Warn($"({_context}) {message}");
    public void Error(string message, Exception ex = null) => _log.Error($"({_context}) {message}", ex);
  }
}