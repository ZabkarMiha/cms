using System.Net;

namespace Core.Exceptions
{
    public class CouldntSendConfirmEmail : BaseException
    {
        public override string Message
        {
            get { return "Couldn't send confirmation email."; }
        }

        public CouldntSendConfirmEmail() { }

        public CouldntSendConfirmEmail(string message)
            : base(message) { }

        public CouldntSendConfirmEmail(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}