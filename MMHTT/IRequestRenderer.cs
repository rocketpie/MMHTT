using MMHTT.Configuration;

namespace MMHTT
{
  public interface IRequestRenderer
  {
    void Initialize(Config config);
    HttpRequestBase Render(RequestDefinition requestDefinition);
  }
}
