using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace MMHTT.RazorTemplates.Tests
{
  [TestClass()]
  public class RazorRendererTests
  {
    static List<string> _directoriesToCleanUp = new List<string>();

    /// <summary>
    /// TODO: Not working, since loaded assembly files cannot be deleted
    /// </summary>
    [TestCleanup()]
    public void Cleanup()
    {
      try
      {
        while (_directoriesToCleanUp.Any())
        {
          var dir = _directoriesToCleanUp.First();

          Directory.Delete(dir, true);
          _directoriesToCleanUp.Remove(dir);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("TestCleanup Error: " + ex.ToString());
      }
    }

    /// <summary>
    /// directories returned by this method get cleaned up by TestCleanup automatically
    /// </summary>
    /// <returns>empty temp directory name</returns>
    string GetManagedTempDirectory()
    {
      var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Substring(0, 8));
      Directory.CreateDirectory(tmpDir);
      _directoriesToCleanUp.Add(tmpDir);
      return tmpDir;
    }

    static Config _modelTemplateNameConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = Guid.NewGuid().ToString(), TemplateString = "@Model.TemplateName" }
      }
    };
    [TestMethod()]
    public void RenderTestModelTemplateName_ShouldRenderTemplateName()
    {
      var target = new RazorRenderer(GetManagedTempDirectory());
      target.Initialize(_modelTemplateNameConfig);

      var actual = target.Render(
        new RequestDefinition()
        {
          TemplateName = _modelTemplateNameConfig.Templates[0].Name,
        },
        new NameValueCollection());

      Assert.IsNotNull(actual);
      Assert.AreEqual(_modelTemplateNameConfig.Templates[0].Name, actual.RequestContent);
    }

    static Config _multiTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "a", TemplateString = "Hi" },
        new Template() { Name = "b", TemplateString = "Ho" }
      }
    };
    [TestMethod()]
    public void RenderTestMultiTemplate_ShouldProduceExpectedResults()
    {
      var target = new RazorRenderer(GetManagedTempDirectory());
      target.Initialize(_multiTemplateConfig);

      var actuala = target.Render(new RequestDefinition() { TemplateName = _multiTemplateConfig.Templates[0].Name }, new NameValueCollection());
      var actualb = target.Render(new RequestDefinition() { TemplateName = _multiTemplateConfig.Templates[1].Name }, new NameValueCollection());

      Assert.IsNotNull(actuala);
      Assert.IsNotNull(actualb);
      Assert.AreEqual(_multiTemplateConfig.Templates[0].TemplateString, actuala.RequestContent);
      Assert.AreEqual(_multiTemplateConfig.Templates[1].TemplateString, actualb.RequestContent);
    }

    /// <summary>
    /// Test Method can be set from inside the template
    /// </summary>
    static Config _setMethodTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "a", TemplateString = "@{Method = \"GET\";}" }
      }
    };
    [TestMethod()]
    public void RenderTestTemplate_MustOverrideRequestMethod()
    {
      var target = new RazorRenderer(GetManagedTempDirectory());
      target.Initialize(_setMethodTemplateConfig);

      var actual = target.Render(new RequestDefinition() { TemplateName = _setMethodTemplateConfig.Templates[0].Name }, new NameValueCollection());
      Assert.IsNotNull(actual);
      Assert.AreEqual("GET", actual.Method);
    }

    /// <summary>
    /// Test Method can be set from inside the template
    /// </summary>
    static Config _setUrlTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "a", TemplateString = "@{Url = \"http://test.de\";}" }
      }
    };
    [TestMethod()]
    public void RenderTestTemplate_MustOverrideRequestUrl()
    {
      var target = new RazorRenderer(GetManagedTempDirectory());
      target.Initialize(_setUrlTemplateConfig);

      var actual = target.Render(new RequestDefinition() { TemplateName = _setUrlTemplateConfig.Templates[0].Name }, new NameValueCollection());

      Assert.IsNotNull(actual);
      Assert.AreEqual("http://test.de", actual.Url);
    }


    /// <summary>
    /// Test Method can be set from inside the template
    /// </summary>
    static Config _renderGuidTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "a", TemplateString = "5fec4a51-f9de-41ba-b696-94e0e519be0c" },
        new Template() { Name = "b", TemplateString = "5fec4a51-f9de-41ba-b696-94e0e519be0c" }
      }
    };
    [TestMethod()]
    public void RenderTestSameTemplate_ShouldCreateOnlyOneAssembly()
    {
      var tmpPath = GetManagedTempDirectory();

      var target = new RazorRenderer(tmpPath);
      Assert.AreEqual(0, Directory.GetFiles(tmpPath).Length);

      target.Initialize(_setMethodTemplateConfig);
      Assert.AreEqual(1, Directory.GetFiles(tmpPath).Length);

      target = new RazorRenderer(tmpPath);
      target.Initialize(_setMethodTemplateConfig);
      Assert.AreEqual(1, Directory.GetFiles(tmpPath).Length);
    }


  }
}