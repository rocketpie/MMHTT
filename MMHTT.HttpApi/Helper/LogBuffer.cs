using System;
using System.Collections.Generic;

namespace MMHTT.HttpApi.Helper
{
  public class LogBuffer : ILog
  {
    public Queue<string> Buffer { get; private set; } = new Queue<string>();

    public void Debug(string message)
    {
      Buffer.Enqueue($"DEBUG: '{message}'");
    }

    public void Info(string message)
    {
      Buffer.Enqueue($"INFO: '{message}'");
    }

    public void Warn(string message)
    {
      Buffer.Enqueue($"WARNING: '{message}'");
    }

    public void Error(string message, Exception ex)
    {
      Buffer.Enqueue($"{DateTime.Now.ToShortTimeString()} ERROR: '{message}' {ex?.ToString()}");
    }
  }
}