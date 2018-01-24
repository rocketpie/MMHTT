using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;
using System;
using System.Collections.Specialized;
using System.IO;

namespace MMHTT.RazorTemplates.Tests
{
  [TestClass()]
  public class RazorRendererTests
  {

    static Config _modelTemplateNameConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = Guid.NewGuid().ToString(), TemplateString = "@Model.TemplateName" }
      }
    };
    [TestMethod()]
    public void RenderTestModelTemplateName_ShouldRenderTemplateName()
    {
      var target = new RazorRenderer(Path.GetTempPath());
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
      var target = new RazorRenderer(Path.GetTempPath());
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
      var target = new RazorRenderer(Path.GetTempPath());
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
      var target = new RazorRenderer(Path.GetTempPath());
      target.Initialize(_setUrlTemplateConfig);

      var actual = target.Render(new RequestDefinition() { TemplateName = _setUrlTemplateConfig.Templates[0].Name }, new NameValueCollection());

      Assert.IsNotNull(actual);
      Assert.AreEqual("http://test.de", actual.Url);
    }
  }
}