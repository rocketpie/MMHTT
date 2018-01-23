using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;

namespace MMHTT.RazorTemplates.Tests
{
  [TestClass()]
  public class RazorRendererTests
  {

    static Config _simpleTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "hi", TemplateString = "Hi, @Model.Agent!" }
      }
    };
    [TestMethod()]
    public void RenderTestSimpleTemplate_ShouldProduceExpectedResult()
    {
      var target = new RazorRenderer();
      target.Initialize(_simpleTemplateConfig);

      var actual = target.Render(new RequestDefinition() { TemplateName = _simpleTemplateConfig.Templates[0].Name, Agent = "World" });

      Assert.IsNotNull(actual);
      Assert.AreEqual("Hi, World!", actual.Result);
    }

    static Config _multiTemplateConfig = new Config()
    {
      Templates = new Template[] {
        new Template() { Name = "hi", TemplateString = "Hi, @Model.Agent!" },
        new Template() { Name = "ho", TemplateString = "Ho, @Model.Agent!" }
      }
    };
    [TestMethod()]
    public void RenderTestMultiTemplate_ShouldProduceExpectedResults()
    {
      var target = new RazorRenderer();
      target.Initialize(_multiTemplateConfig);

      var actuala = target.Render(new RequestDefinition() { TemplateName = _multiTemplateConfig.Templates[0].Name, Agent = "World" });
      var actualb = target.Render(new RequestDefinition() { TemplateName = _multiTemplateConfig.Templates[1].Name, Agent = "World" });

      Assert.IsNotNull(actuala);
      Assert.IsNotNull(actualb);
      Assert.AreEqual("Hi, World!", actuala.Result);
      Assert.AreEqual("Ho, World!", actualb.Result);
    }
  }
}