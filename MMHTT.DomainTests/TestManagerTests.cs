﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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