using System;

namespace MMHTT.Domain
{
  public class ConsoleLog : ILog
  {
    public void Debug(string message)
    {
      Console.WriteLine($"DEBUG: '{message}'");
    }

    public void Info(string message)
    {
      Console.WriteLine($"INFO: '{message}'");
    }

    public void Warn(string message)
    {
      Console.WriteLine($"WARNING: '{message}'");
    }

    public void Error(string message, Exception ex)
    {
      Console.WriteLine($"{DateTime.Now.ToShortTimeString()} ERROR: '{message}' {ex?.ToString()}");
    }
  }
}