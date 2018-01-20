using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MMHTT.Domain.Tests
{
  [TestClass()]
  public class TestManagerTests
  {         
    static Settings _insufficientSettingsTest = new Settings()
    {
      Templates = new Template[] { },
      RequestVariations = new RequestVariation[]
     {
        new RequestVariation() { }
     }
    };

    /// <summary>
    /// details tested by SettingsManagerTest
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(SettingsException))]
    public void TestManagerParseTestInsufficientSettings_ShouldThrowSettingsException()
    {
      var target = TestManager.Parse(_insufficientSettingsTest);
    }

    [TestMethod()]
    public void StartTest()
    {
      Assert.Fail();
    }

    [TestMethod()]
    public void StopTest()
    {
      Assert.Fail();
    }
  }
}