using System;

namespace MMHTT.Domain
{
  class NullLog : ILog
  {
    public void Error(string message, Exception ex)
    {
    }
  }
}