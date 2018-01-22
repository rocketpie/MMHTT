using System.ComponentModel;
using System.Text;

namespace MMHTT.RazorTemplates
{
  public abstract class HttpRequestTemplateBase : HttpRequestBase
  {
    /// <summary>
    /// This is where output is written to
    /// </summary>
    [Browsable(false)]
    public StringBuilder Buffer { get; set; }

    public HttpRequestTemplateBase()
    {
      Buffer = new StringBuilder();
    }

    public abstract void Execute();

    // Writes the results of expressions like: "@foo.Bar"
    public virtual void Write(object value)
    {
      // Don't need to do anything special
      // Razor for ASP.Net does HTML encoding here.
      WriteLiteral(value);
    }

    // Writes literals like markup: "<p>Foo</p>"
    public virtual void WriteLiteral(object value)
    {
      Buffer.Append(value);
    }
  }
}
