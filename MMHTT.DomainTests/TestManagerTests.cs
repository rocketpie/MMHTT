using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;

namespace MMHTT.Domain.Tests
{
  [TestClass()]
  public class TestManagerTests
  {
    static Config _insufficientSettingsTest = new Config()
    {
      Templates = new Template[] { },
      RequestDefinitions = new RequestDefinition[]
     {
        new RequestDefinition() { }
     }
    };
    [TestMethod()]
    [ExpectedException(typeof(SettingsException))]
    public void TestManagerParseTestInsufficientSettings_ShouldThrowSettingsException()
    {
      var target = TestManager.Initialize(_insufficientSettingsTest, null);
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