using System.Net;

namespace Core.Exceptions
{
    public class UserNotFoundException : BaseException
    {
        public override string Message
        {
            get { return "Couldn't find user(s)."; }
        }

        public UserNotFoundException() { }

        public UserNotFoundException(string message)
            : base(message) { }

        public UserNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
