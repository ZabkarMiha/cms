using System.Net;

namespace Core.Exceptions
{
    public class ProfileException : BaseException
    {
        public ProfileException() { }

        public ProfileException(string message)
            : base(message) { }

        public ProfileException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}
