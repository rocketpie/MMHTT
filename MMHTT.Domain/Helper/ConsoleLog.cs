using System;

namespace MMHTT.Domain
{
  public class ConsoleLog : ILog
  {
    public void Error(string message, Exception ex)
    {
      Console.WriteLine($"{DateTime.Now.ToShortTimeString()} ERROR: '{message}' {ex?.ToString()}");
    }

    public void Warn(string message)
    {
      Console.WriteLine($"WARNING: '{message}'");
    }
  }
}