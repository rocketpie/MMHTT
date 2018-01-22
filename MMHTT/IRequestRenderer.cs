using MMHTT.Configuration;
using System.Net.Http;

namespace MMHTT
{
  public interface IRequestRenderer
  {
    HttpRequestMessage Render(RequestDefinition variation);

  }
}
