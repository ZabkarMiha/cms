using System.Net;

namespace Core.Exceptions
{
    public class EmptyInputException : BaseException
    {
        private string FieldName { get; }
        public override string Message
        {
            get { return "Input field " + FieldName + " must not be empty."; }
        }

        public EmptyInputException() { }

        public EmptyInputException(string message)
            : base(message) { 
                FieldName = message;
            }

        public EmptyInputException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}
