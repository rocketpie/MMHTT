using MMHTT.Configuration;
using System.Collections.Specialized;

namespace MMHTT
{
  public interface IRequestRenderer
  {
    void Initialize(Config config);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestDefinition"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    HttpRequestBase Render(RequestDefinition requestDefinition, NameValueCollection session);
  }
}
