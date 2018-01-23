using MMHTT.Configuration;
using MMHTT.Domain;
using MMHTT.HttpApi.Helper;
using MMHTT.HttpApi.Models;
using MMHTT.RazorTemplates;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace MMHTT.HttpApi.Controllers
{
  public class TestController : ApiController
  {
    Dictionary<string, TestModel> _tests = new Dictionary<string, TestModel>();

    // GET api/values
    public IEnumerable<string> Get()
    {
      return _tests.Keys;
    }

    // GET api/values/5
    public string[] Get(string id)
    {
      if (!_tests.ContainsKey(id)) { throw new HttpResponseException(System.Net.HttpStatusCode.NotFound); }
      return _tests[id].Log.Buffer.ToArray();
    }

    // PUT api/values/5
    public void Put(string name, [FromBody]Config config)
    {
      string id = Guid.NewGuid().ToString("N");

      var logbuffer = new LogBuffer();
      _tests.Add(id, new TestModel() { Id = id, Name = name, TestManager = TestManager.ParseAndInitialize(config, new RazorRenderer(), logbuffer), Log = logbuffer });
    }

    // DELETE api/values/5
    public void Delete(string id)
    {
      if (!_tests.ContainsKey(id)) { throw new HttpResponseException(System.Net.HttpStatusCode.NotFound); }

      _tests[id].TestManager.Abort();
      _tests.Remove(id);
    }
  }
}
