using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMHTT.RazorTemplates.Tests
{
  [TestClass()]
  public class RazorRendererTests
  {
    [TestMethod()]
    public void RenderTestSimpleTemplate_ShouldProduceExpectedResult()
    {
      var renderer = RazorRenderer.Parse("Hi, @Model!");
      Assert.AreEqual("Hi, world!", renderer.Render("world"));
    }
  }
}