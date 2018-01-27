using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestAPI.Controllers
{
  public class StatusCodeController : ApiController
  {
    [Route("api/statuscode/{code}")]
    public HttpResponseMessage Get(int code, string body = null)
    {
      LogController.Log.Enqueue("1");
      StatusController.CountCall();

      if (string.IsNullOrWhiteSpace(body))
      {
        return Request.CreateResponse((HttpStatusCode)code);
      }
      else
      {
        return Request.CreateResponse((HttpStatusCode)code, body);
      }
    }

    [Route("api/statuscode/{code}")]
    public HttpResponseMessage Post(int code, string body = null)
    {
      if (string.IsNullOrWhiteSpace(body))
      {
        return Request.CreateResponse((HttpStatusCode)code);
      }
      else
      {
        return Request.CreateResponse((HttpStatusCode)code, body);
      }
    }

    [Route("api/statuscode/{code}")]
    public HttpResponseMessage Put(int code, string body = null)
    {
      if (string.IsNullOrWhiteSpace(body))
      {
        return Request.CreateResponse((HttpStatusCode)code);
      }
      else
      {
        return Request.CreateResponse((HttpStatusCode)code, body);
      }
    }

    [Route("api/statuscode/{code}")]
    public HttpResponseMessage Delete(int code, string body = null)
    {
      if (string.IsNullOrWhiteSpace(body))
      {
        return Request.CreateResponse((HttpStatusCode)code);
      }
      else
      {
        return Request.CreateResponse((HttpStatusCode)code, body);
      }
    }
  }
}