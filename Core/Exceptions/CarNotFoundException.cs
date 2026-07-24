using System.Net;

namespace Core.Exceptions
{
    public class CarNotFoundException : BaseException
    {
        public override string Message
        {
            get { return "Couldn't find car(s)."; }
        }

        public CarNotFoundException() { }

        public CarNotFoundException(string message)
            : base(message) { }

        public CarNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
