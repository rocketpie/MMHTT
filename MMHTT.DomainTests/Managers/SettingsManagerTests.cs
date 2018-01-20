using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace MMHTT.Domain.Tests
{
  [TestClass()]
  public class SettingsManagerTests
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

    string GetTempFileManaged()
    {
      var tmp = Path.GetTempFileName();
      _filesToCleanUp.Add(tmp);
      return tmp;
    }

    void AssertAreEqual(Settings expected, Settings actual)
    {
      if (expected == null) { Assert.IsNull(actual); return; }
      Assert.IsNotNull(actual);

      Assert.AreEqual(expected.MaxTotalRequests, actual.MaxTotalRequests);
      Assert.AreEqual(expected.Renderer, actual.Renderer);
      Assert.AreEqual(expected.RequestVariationsFile, actual.RequestVariationsFile);

      AssertAreEqual(expected.AgentBehaviour, actual.AgentBehaviour);
      AssertAreEqual(expected.Templates, actual.Templates);
      AssertAreEqual(expected.RequestVariations, actual.RequestVariations);
    }

    private void AssertAreEqual(RequestVariation[] expected, RequestVariation[] actual)
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
    /// write / read test: all properties set
    /// </summary>
    static Settings _FullSettingsTestA = new Settings()
    {
      AgentBehaviour = new AgentBehaviour[] {
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
      RequestVariations = new RequestVariation[] {
        new RequestVariation() {
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
      Renderer = "tiraen",
      RequestVariationsFile = "atreunitae@\fgvxh435."
    };


    /// <summary>
    /// request variations and TemplateStrings in seperate files.
    /// </summary>
    static Settings _settingsFilesTest = new Settings()
    {
      RequestVariationsFile = "../../TestFiles/RequestVariations",
      Templates = new Template[] {
        new Template() { Name = "GetUser", File = "../../TestFiles/GetUser.request" } },
      MaxTotalRequests = 100,
    };

    /// <summary>
    /// minimum info to start a test
    /// </summary>
    static Settings _minimalSettingsTest = new Settings()
    {
      Templates = new Template[] {
       new Template() { Name = "t1", TemplateString ="" }
      },
      RequestVariations = new RequestVariation[]
      {
        new RequestVariation() { Endpoint = "http://example.com" , TemplateName = "t1" }
      }
    };

    [TestMethod()]
    public void LoadAndTestMinimalSettings_ShouldNotFail()
    {
      SettingsManager.LoadAndTest(_minimalSettingsTest);
    }

    [TestMethod()]
    public void LoadAndTestSettingsFiles_ShouldNotFail()
    {
      SettingsManager.LoadAndTest(_settingsFilesTest);
    }

    [TestMethod()]
    public void WriteAndRead_ResultShouldEqualTestData()
    {
      var tmpfile = GetTempFileManaged();

      SettingsManager.SaveFile(_FullSettingsTestA, tmpfile);
      var actual = SettingsManager.ReadFromFile(tmpfile);

      AssertAreEqual(_FullSettingsTestA, actual);
    }

    [TestMethod()]
    public void SaveFileTest_ShouldNotFail()
    {
      var file = GetTempFileManaged();
      SettingsManager.SaveFile(_minimalSettingsTest, file);
    }
  }
}