using MMHTT.Configuration;
using MMHTT.Domain;
using MMHTT.HttpApi.Helper;
using MMHTT.HttpApi.Models;
using MMHTT.RazorTemplates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

namespace MMHTT.HttpApi.Controllers
{
  public class TestController : ApiController
  {
    const string ASSEMBLY_PATH_SETTING = "AssemblyPath";

    // GET api/values
    public IEnumerable<string> Get() => TestRepository.GetAllKeys();


    // GET api/values/5
    public string[] Get(string id)
    {
      var test = TestRepository.Get(id);

      var data = test.Log.Buffer.ToArray();
      test.Log.Buffer.Clear();

      return data;
    }

    // PUT api/values/5
    public string Put(string name, [FromBody]Config config)
    {
      string id = Guid.NewGuid().ToString("N");
      var logbuffer = new LogBuffer();

      TestManager testManager = TestManager.Initialize(config, new RazorRenderer(ConfigurationManager.AppSettings[ASSEMBLY_PATH_SETTING]), logbuffer);

      TestRepository.Register(new TestModel() { Id = id, Name = name, Log = logbuffer, TestManager = testManager });

      testManager.Start();

      return id;
    }

    // DELETE api/values/5
    public void Delete(string id)
    {
      TestRepository.Get(id).TestManager.Abort();
      TestRepository.Delete(id);
    }

  }
}