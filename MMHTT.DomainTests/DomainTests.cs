using System;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMHTT.Configuration;
using MMHTT.Domain;

namespace MMHTT.DomainTests
{
  /// <summary>
  /// General Domain behaviour Tests
  /// </summary>
  [TestClass]
  public class DomainTests
  {
    private class TestRenderer : IRequestRenderer
    {
      public delegate void Initializer(Config config);
      public delegate HttpRequestBase Renderer(RequestDefinition requestDefinition, NameValueCollection session);

      private Initializer _initializer;
      private Renderer _renderer;

      public TestRenderer(Initializer initializer, Renderer renderer)
      {
        _initializer = initializer;
        _renderer = renderer;
      }

      public void Initialize(Config config) => _initializer(config);
      public HttpRequestBase Render(RequestDefinition requestDefinition, NameValueCollection session) => _renderer(requestDefinition, session);
    }

    /// <summary>
    /// we define a 'Default' agent with initial data.
    /// we define a request with an unspecified agent
    /// we expect the unspecified agent to inherit the 'Default' agent's initial data
    /// </summary>
    Config _defaultAgentConfig = new Config
    {
      AgentBehaviours = new AgentBehaviour[] {
        new AgentBehaviour(){
          Agent = "Default",
          InitialSessionData = new KeyValue[] {
            new KeyValue(){ Key = "sessionId", Value = Guid.NewGuid().ToString() }
          }
        }
      },
      RequestDefinitions = new RequestDefinition[] {
        new RequestDefinition(){ Agents = new string[]{ "1" }, TemplateName = "t" }
      },
      Templates = new Template[] {
        new Template() { Name = "t", TemplateString = "t" }
      },
      MaxRequestsPerSecond = 1000,
      MaxTestRuntimeSeconds = 1000,
      MaxTotalRequests = 1000
    };
    [TestMethod]
    public void DefaultAgentBehaviour_ShouldBeUsedForAgentsWithoutSpecificBehaviour()
    {
      var agentHasDefaultAgentInitialSessionData = false;

      var wait = new ManualResetEvent(false);

      var target = TestManager.Initialize(
           _defaultAgentConfig,
           new TestRenderer(
             c => { },
             (r, s) =>
             {
               /// if the agent "1" implements the 'Default' agents InitialSessionData, all is well
               if (s[_defaultAgentConfig.AgentBehaviours[0].InitialSessionData[0].Key] == _defaultAgentConfig.AgentBehaviours[0].InitialSessionData[0].Value)
               {
                 agentHasDefaultAgentInitialSessionData = true;
               }
               wait.Set();
               return null;
             }
      ));

      try
      {
        target.Start();
      }
      catch (Exception ex)
      {
        throw;
      }

      wait.WaitOne(1000);

      Assert.IsTrue(agentHasDefaultAgentInitialSessionData);
    }
  }
}
