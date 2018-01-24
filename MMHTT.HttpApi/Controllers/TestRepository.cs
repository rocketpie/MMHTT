using MMHTT.HttpApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MMHTT.HttpApi.Controllers
{
  public static class TestRepository
  {
    static Dictionary<string, TestModel> _repository = new Dictionary<string, TestModel>();

    public static void Register(TestModel test)
    {
      _repository.Add(test.Id, test);
    }

    public static string[] GetAllKeys()
    {
      return _repository.Keys.ToArray();
    }

    public static TestModel Get(string id)
    {
      if (!_repository.ContainsKey(id))
      {
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }

      return _repository[id];
    }

    public static void Delete(string id)
    {
      if (!_repository.ContainsKey(id))
      {
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }

      _repository.Remove(id);
    }
  }
}