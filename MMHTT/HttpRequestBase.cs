using MMHTT.Configuration;
using System;
using System.Collections.Specialized;
using System.Net.Http;

namespace MMHTT
{
  /// <summary>
  /// Template Rendering Base class.
  /// 
  /// * Use Model and Agent inside the template to write data into the Request
  /// * Set Method and Url from inside the template
  /// * Add additional Http request Headers to the 'Headers' NameValueCollection
  /// * Add 
  /// 
  /// </summary>
  public abstract class HttpRequestBase
  {
    /// <summary>
    /// gets set before Execute, to be used in the template
    /// </summary>
    public RequestDefinition Model { get; set; }

    /// <summary>
    /// Agent specific NameValueCollection 
    /// </summary>
    public NameValueCollection Session { get; set; }

    /// <summary>
    /// expected to be set after execute
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// expected to be set after execute
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// may be set after execute
    /// </summary>
    public NameValueCollection Headers { get; set; } = new NameValueCollection();

    /// <summary>
    /// Rendering must write the rendered Output to this Field
    /// </summary>
    public string RequestContent { get; set; }

    /// <summary>
    /// Set this to execute some code after the request completed, like setting Session data from the response
    /// args: request (this), response, session 
    /// </summary>
    public Action<HttpRequestBase, HttpResponseMessage, NameValueCollection> OnResponse { get; set; }

  }
}