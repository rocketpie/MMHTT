using System.ComponentModel;
using System.IO;
using System.Text;

namespace MMHTT.RazorTemplates
{

  /// <summary>
  /// TODO: 
  /// 1 put into MMHTT/
  /// 2 Add properties for creating request: Method, Headers, ...
  /// 3 Put RequestVariation Model into Execute?
  /// </summary>
  public abstract class TemplateBase
  {
    [Browsable(false)]
    public StringBuilder Buffer { get; set; }

    [Browsable(false)]
    public StringWriter Writer { get; set; }

    public TemplateBase()
    {
      Buffer = new StringBuilder();
      Writer = new StringWriter(Buffer);
    }

    public dynamic Model { get; set; }

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
