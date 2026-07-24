using System.Net;

namespace Core.Exceptions
{
    public class KeyAlreadyExistsException : BaseException
    {
        public string Details { get; }
        public override string Message
        {
            get { return Details; }
        }

        public KeyAlreadyExistsException() { }

        public KeyAlreadyExistsException(string message)
            : base(message)
        {
            Details = message;
        }

        public KeyAlreadyExistsException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}