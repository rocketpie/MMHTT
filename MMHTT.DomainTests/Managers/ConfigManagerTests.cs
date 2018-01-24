using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;
using System.Collections.Generic;
using System.IO;

namespace MMHTT.Domain.Tests
{
  [TestClass()]
  public class ConfigManagerTests
  {
    [TestInitialize()]
    public void Initialize()
    {
    }

    static List<string> _filesToCleanUp = new List<string>();

    [TestCleanup()]
    public void Cleanup()
    {
      foreach (var file in _filesToCleanUp)
      {
        try
        {
          File.Delete(file);
        }
        catch { }
      }
    }

    /// <summary>
    /// files returned by this method get cleaned up by TestCleanup automatically
    /// </summary>
    /// <returns></returns>
    string GetTempFileManaged()
    {
      var tmp = Path.GetTempFileName();
      _filesToCleanUp.Add(tmp);
      return tmp;
    }

    void AssertAreEqual(Config expected, Config actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.MaxTotalRequests, actual.MaxTotalRequests);
      Assert.AreEqual(expected.MaxRequestsPerSecond, actual.MaxRequestsPerSecond);
      Assert.AreEqual(expected.MaxTestRuntimeSeconds, actual.MaxTestRuntimeSeconds);
      Assert.AreEqual(expected.RequestDefinitionsFile, actual.RequestDefinitionsFile);

      AssertAreEqual(expected.AgentBehaviours, actual.AgentBehaviours);
      AssertAreEqual(expected.Templates, actual.Templates);
      AssertAreEqual(expected.RequestDefinitions, actual.RequestDefinitions);
    }

    private void AssertAreEqual(RequestDefinition[] expected, RequestDefinition[] actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
      {
        Assert.AreEqual(expected[i].Agent, actual[i].Agent);
        Assert.AreEqual(expected[i].Weight, actual[i].Weight);
        Assert.AreEqual(expected[i].Endpoint, actual[i].Endpoint);
        Assert.AreEqual(expected[i].TemplateName, actual[i].TemplateName);

        AssertAreEqual(expected[i].KeyValues, actual[i].KeyValues);
      }
    }

    private void AssertAreEqual(KeyValue[] expected, KeyValue[] actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
      {
        Assert.AreEqual(expected[i].Key, actual[i].Key);
        Assert.AreEqual(expected[i].Value, actual[i].Value);
      }
    }

    private void AssertAreEqual(Template[] expected, Template[] actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
      {
        Assert.AreEqual(expected[i].File, actual[i].File);
        Assert.AreEqual(expected[i].Name, actual[i].Name);
        Assert.AreEqual(expected[i].TemplateString, actual[i].TemplateString);
      }
    }

    private void AssertAreEqual(AgentBehaviour[] expected, AgentBehaviour[] actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
      {
        Assert.AreEqual(expected[i].Agent, actual[i].Agent);
        Assert.AreEqual(expected[i].MaxRequestsPerSecond, actual[i].MaxRequestsPerSecond);
      }
    }


    /// <summary>
    /// minimum info to start a test
    /// </summary>
    static Config _minimalConfig = new Config()
    {
      Templates = new Template[] {
       new Template() { Name = "t1", TemplateString ="" }
      },
      RequestDefinitions = new RequestDefinition[]
      {
        new RequestDefinition() { Endpoint = "http://example.com" , TemplateName = "t1" }
      }
    };
    [TestMethod()]
    public void LoadAndTestMinimalConfig_ShouldNotFail()
    {
      ConfigManager.LoadAndTest(_minimalConfig);
    }
    [TestMethod()]
    public void SaveFileTest_ShouldNotFail()
    {
      var file = GetTempFileManaged();
      ConfigManager.SaveFile(_minimalConfig, file);
    }


    /// <summary>
    /// request variations and TemplateStrings in seperate files.
    /// </summary>
    static Config _FilesConfig = new Config()
    {
      RequestDefinitionsFile = "../../TestFiles/RequestDefinitions",
      Templates = new Template[] {
        new Template() { Name = "GetUser", File = "../../TestFiles/GetUser.request" } },
      MaxTotalRequests = 100,
    };
    [TestMethod()]
    public void LoadAndTestConfigFiles_ShouldNotFail()
    {
      ConfigManager.LoadAndTest(_FilesConfig);
    }


    /// <summary>
    /// write / read test: all properties set
    /// </summary>
    static Config _AllFieldsSetConfig = new Config()
    {
      AgentBehaviours = new AgentBehaviour[] {
         new AgentBehaviour() {
           Agent = "a",
           MaxRequestsPerSecond = 1345
         }
       },
      Templates = new Template[] {
        new Template() {
          Name = "GetUser",
          File = "GetUser.request",
          TemplateString = "test data .@982746’‘‚  98¥‘‚¢‚²‘‚¥¹³‘‚‚¥[<?}ai" }
      },
      RequestDefinitions = new RequestDefinition[] {
        new RequestDefinition() {
          Agent ="1",
          Endpoint = "localhost/service.svc",
          TemplateName = "GetUser",
          Weight = 30000,
          KeyValues = new KeyValue[] {
            new KeyValue() { Key = "SID", Value= "00166228051029914098B1F5DB42A7FC3FA4001" },
          }
        },
      },
      MaxTotalRequests = 100,
      MaxRequestsPerSecond = 20,
      MaxTestRuntimeSeconds = 6788,
      RequestDefinitionsFile = "atreunitae@\fgvxh435."
    };
    [TestMethod()]
    public void WriteAndRead_ResultShouldEqualTestData()
    {
      var tmpfile = GetTempFileManaged();

      ConfigManager.SaveFile(_AllFieldsSetConfig, tmpfile);
      var actual = ConfigManager.ReadFromFile(tmpfile);

      AssertAreEqual(_AllFieldsSetConfig, actual);
    }


    /// <summary>
    /// minimum info to start a test
    /// </summary>
    static Config _defaultBehaviourConfig = new Config()
    {
      Templates = new Template[] {
       new Template() { Name = "t1", TemplateString ="" }
      },
      RequestDefinitions = new RequestDefinition[]
      {
        new RequestDefinition() { Endpoint = "http://example.com" , TemplateName = "t1" }
      },
      AgentBehaviours = new AgentBehaviour[] {
        new AgentBehaviour() { Agent = "Default", MaxRequestsPerSecond = 30 }
      }
    };
    [TestMethod()]
    public void LoadAndTestDefaultBehaviourConfig_ShouldOverrideAgentBehaviourDefault()
    {
      ConfigManager.LoadAndTest(_defaultBehaviourConfig);
      Assert.Inconclusive(); // how to test now?      
    }



  }
}