using System.Net;

namespace Core.Exceptions
{
    public class CarEnumNotDefinedException : BaseException
    {
        public string EnumName { get; }
        public override string Message
        {
            get { return "Incorrect value for " + EnumName; }
        }

        public CarEnumNotDefinedException() { }

        public CarEnumNotDefinedException(string message)
            : base(message)
        {
            EnumName = message;
        }

        public CarEnumNotDefinedException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}
