# MMHTT - Massively Multithreaded Http Testing Tool

or massively multithreaded online request procedural generator? 

The Idea is to provide request definitions (request to make) with the following data structure:

* Config 
  * -> RequestDefinition
    * -> Template
    * -> KeyValues (dynamic data) 
    * -> Request agent behaviour

example:  
{
  "AgentBehaviours": [
    {
      "Agent":"Default",
      "InitialSessionData":[
        { "Key":"sessionId", "Value":"abc" }
      ]
    }
  ],
  "RequestDefinitions":[
    {
      "Agents":[ "1", "2", "3" ],
      "TemplateName":"getSomeData"
    }
  ],
  "Templates":[
    {
      "Name":"getSomeData",
      "TemplateString":"@{Url = \"http://example.com\"; Method = \"GET\"; }"
    }
  ],
  "MaxTotalRequests":100,
  "MaxRequestsPerSecond":3,
  "MaxTestRuntimeSeconds":10,
}

# TODOs:
read KeyValues from file(s) in Sets, so that random/generic test data can be provided en masse via file.
Then select from that data sequentially/randomly ? 
