using System.Net;

namespace Core.Requests
{
    public class ErrorResponseRequest
    {
        public string Message;
        public HttpStatusCode StatusCode;
    }
}