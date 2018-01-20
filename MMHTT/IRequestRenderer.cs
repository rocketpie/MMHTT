using System.Net.Http;

namespace MMHTT
{
  public interface IRequestRenderer
  {
    HttpRequestMessage Render(RequestVariation variation);

  }
}
