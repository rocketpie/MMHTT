using MMHTT.Configuration;
using System.Collections.Specialized;

namespace MMHTT
{
  /// <summary>
  /// Template Rendering Output Base class
  /// </summary>
  public abstract class HttpRequestBase
  {
    /// <summary>
    /// gets set before Execute, to be used in the template
    /// </summary>
    public RequestDefinition Model { get; set; }

    /// <summary>
    /// expected to be set after execute
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// may be set after execute
    /// </summary>
    public NameValueCollection Headers { get; set; } = new NameValueCollection();

    /// <summary>
    /// Rendering will write the rendered Output to this Field
    /// </summary>
    public string Result { get; set; }

  }
}