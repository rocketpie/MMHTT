using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestAPI.Controllers
{
  public class LogController : ApiController
  {
    public static ConcurrentQueue<string> Log { get; private set; } = new ConcurrentQueue<string>();

    [Route("api/log")]
    public string[] Get()
    {
      return Log.ToArray();
    }
  }
}
