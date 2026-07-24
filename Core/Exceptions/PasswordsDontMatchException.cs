using System.Net;

namespace Core.Exceptions
{
    public class PasswordsDontMatchException : BaseException
    {
        public override string Message
        {
            get { return "Password and ConfirmPassword don't match"; }
        }
        public PasswordsDontMatchException() { }

        public PasswordsDontMatchException(string message)
            : base(message) { }

        public PasswordsDontMatchException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}