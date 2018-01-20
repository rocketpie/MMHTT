using System;

namespace MMHTT
{
  public interface ILog
  {
    void Error(string message, Exception ex = null);
  }
}