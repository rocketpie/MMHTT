using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;

namespace MMHTT.RazorTemplates.Tests
{
  [TestClass()]
  public class RazorRendererTests
  {
    [TestMethod()]
    public void RenderTestSimpleTemplate_ShouldProduceExpectedResult()
    {
      var target = RazorRenderer.Parse("Hi, @Model.Agent!");
      var actual = target.Render(new RequestDefinition() { Agent = "World" });

      Assert.IsNotNull(actual);
      Assert.AreEqual("Hi, World!", actual.Result);
    }
  }
}