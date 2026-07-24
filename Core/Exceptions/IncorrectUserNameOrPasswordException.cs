using System.Net;

namespace Core.Exceptions
{
    public class IncorrectUserNameOrPasswordException : BaseException
    {
        public override string Message
        {
            get { return "Incorrect UserName or Password."; }
        }

        public IncorrectUserNameOrPasswordException() { }

        public IncorrectUserNameOrPasswordException(string message)
            : base(message) { }

        public IncorrectUserNameOrPasswordException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}
