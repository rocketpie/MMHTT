using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestAPI.Controllers
{
  public class StatusCodeController : ApiController
  {
    public HttpResponseMessage Get(int code, string body)
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

    public HttpResponseMessage Post(int code, string body)
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

    public HttpResponseMessage Put(int code, string body)
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

    public HttpResponseMessage Delete(int code, string body)
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