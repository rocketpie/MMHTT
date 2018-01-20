using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;

namespace MMHTT.RazorTemplates
{
  public class RazorRenderer
  {
    static string _generatingAssemblyPath = typeof(RazorRenderer).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\");
    const string GENERATED_NAMESPACE = "RazorOutput";
    const string GENERATED_TEMPLATE_TYPE = "Template";

    private RazorRenderer() { }

    /// <summary>
    /// compiler
    /// </summary>
    CSharpCodeProvider _codeProvider = new CSharpCodeProvider();
    TemplateBase _templateInstance;

    /// <summary>
    /// generate Razor assembly
    /// </summary>
    /// <param name="template"></param>
    public static RazorRenderer Parse(string template)
    {
      var result = new RazorRenderer();

      // Set up the hosting environment         
      // a. Use the C# language (you could detect this based on the file extension if you want to)
      RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
      host.DefaultBaseClass = typeof(TemplateBase).FullName;

      // c. Set the output namespace and type name
      host.DefaultNamespace = GENERATED_NAMESPACE;
      host.DefaultClassName = GENERATED_TEMPLATE_TYPE;

      // d. Add default imports and references
      host.NamespaceImports.Add("System");
      host.NamespaceImports.Add("Microsoft.CSharp");

      CompilerParameters compilerParameters = new CompilerParameters();
      compilerParameters.ReferencedAssemblies.Add(_generatingAssemblyPath); // TemplateBase
      compilerParameters.ReferencedAssemblies.Add("System.dll");
      compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
      compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");   // dynamic support

      string outputAssemblyFile = string.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N"));
      compilerParameters.OutputAssembly = outputAssemblyFile;

      // Create the template engine using this host
      var engine = new RazorTemplateEngine(host);

      // Generate render code from template
      GeneratorResults razorResult = null;
      using (TextReader rdr = new StringReader(template))
      {
        razorResult = engine.GenerateCode(rdr);
      }

      CompilerResults results = result._codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

      if (results.Errors.HasErrors)
      {
        CompilerError complierError = results.Errors
                                   .OfType<CompilerError>()
                                   .Where(ce => !ce.IsWarning)
                                   .First();

        throw new SettingsException($"Error Compiling Template ({complierError.Line}, {complierError.Column}): {complierError.ErrorText}");
      }

      // Load the assembly
      Assembly generatedAssembly = Assembly.LoadFrom(outputAssemblyFile);
      if (generatedAssembly == null)
      {
        throw new Exception("cannot load template assembly (generatedAssembly is null)");
      }

      // Get the template type
      Type templateType = generatedAssembly.GetType($"{GENERATED_NAMESPACE}.{GENERATED_TEMPLATE_TYPE}");
      if (templateType == null)
      {
        throw new Exception("cannot find template Type in assembly (templateType is null)");
      }

      result._templateInstance = Activator.CreateInstance(templateType) as TemplateBase;
      if (result._templateInstance == null)
      {
        throw new Exception("cannot construct template Type (templateInstance is null)");
      }

      return result;
    }

    public string Render(dynamic model = null)
    {
      _templateInstance.Model = model;

      _templateInstance.Execute();
      var result = _templateInstance.Buffer.ToString();
      _templateInstance.Buffer.Clear();

      return result;
    }





  }
}
