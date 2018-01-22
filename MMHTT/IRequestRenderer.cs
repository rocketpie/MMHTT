using MMHTT.Configuration;

namespace MMHTT
{
  public interface IRequestRenderer
  {
    HttpRequestBase Render(RequestDefinition requestDefinition);
  }
}
