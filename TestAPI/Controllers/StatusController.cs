using System.Threading;
using System.Web.Http;

namespace TestAPI.Controllers
{
  public class StatusController : ApiController
  {
    static volatile int _callcnt;

    public static void CountCall()
    {
      Interlocked.Increment(ref _callcnt);
    }

    [Route("api/status")]
    public string Get()
    {
      return _callcnt.ToString();
    }
  }
}