﻿using Microsoft.CSharp;
using MMHTT.Configuration;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Razor;

namespace MMHTT.RazorTemplates
{
  public class RazorRenderer : IRequestRenderer
  {
    /// <summary>
    /// to include in razor includes
    /// </summary>
    static readonly string TEMPLATE_BASE_ASSEMBLY = typeof(HttpRequestBase).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\");
    static readonly string TEMPLATE_ASSEMBLY = typeof(HttpRequestTemplateBase).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\");
    const string GENERATED_NAMESPACE = "RazorOutput";
    const string GENERATED_TEMPLATE_TYPE = "Template";

    public Type TemplateType { get; private set; }

    /// <summary>
    /// generate Razor assembly
    /// </summary>
    /// <param name="template"></param>
    public static RazorRenderer Parse(string template)
    {
      // Set up the hosting environment         
      // a. Use the C# language (you could detect this based on the file extension if you want to)
      RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
      host.DefaultBaseClass = typeof(HttpRequestTemplateBase).FullName;

      // c. Set the output namespace and type name
      host.DefaultNamespace = GENERATED_NAMESPACE;
      host.DefaultClassName = GENERATED_TEMPLATE_TYPE;

      // d. Add default imports and references
      host.NamespaceImports.Add("System");
      host.NamespaceImports.Add("Microsoft.CSharp");

      CompilerParameters compilerParameters = new CompilerParameters();
      compilerParameters.ReferencedAssemblies.Add(TEMPLATE_BASE_ASSEMBLY);
      compilerParameters.ReferencedAssemblies.Add(TEMPLATE_ASSEMBLY);
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

      var codeProvider = new CSharpCodeProvider();
      CompilerResults results = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

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
        throw new Exception($"cannot load template assembly ({nameof(generatedAssembly)} is null)");
      }

      // Get the template type
      Type templateType = generatedAssembly.GetType($"{GENERATED_NAMESPACE}.{GENERATED_TEMPLATE_TYPE}");
      if (templateType == null)
      {
        throw new Exception($"cannot find template Type in assembly ({nameof(templateType)} is null)");
      }

      var testInstance = Activator.CreateInstance(templateType) as HttpRequestTemplateBase;
      if (testInstance == null)
      {
        throw new Exception($"cannot construct template Type ({nameof(testInstance)} is null)");
      }

      return new RazorRenderer() { TemplateType = templateType };
    }


   public HttpRequestBase Render(RequestDefinition requestDefinition)
    {
      var instance = Activator.CreateInstance(TemplateType) as HttpRequestTemplateBase;
      instance.Model = requestDefinition;

      instance.Execute();

      instance.Result = instance.Buffer.ToString();
      instance.Buffer = null;

      return instance;    
    }
  }
}