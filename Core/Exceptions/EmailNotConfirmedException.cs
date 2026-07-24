using System.Net;

namespace Core.Exceptions
{
    public class EmailNotConfirmedException : BaseException
    {
        public override string Message
        {
            get { return "Email not confirmed."; }
        }

        public EmailNotConfirmedException() { }

        public EmailNotConfirmedException(string message)
            : base(message) { }

        public EmailNotConfirmedException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}