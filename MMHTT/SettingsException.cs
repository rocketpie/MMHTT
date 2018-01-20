using System;

namespace MMHTT
{
  public class SettingsException : Exception
  {
    public SettingsException(string message = null, Exception innerExcption = null) : base(message, innerExcption) { }
  }
}
