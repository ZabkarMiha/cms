using System.Net;

namespace Core.Exceptions
{
    public class UserWithCarDoesntExistException : BaseException
    {
        public override string Message
        {
            get { return "Selected user with assigned car doesn't exist "; }
        }

        public UserWithCarDoesntExistException() { }

        public UserWithCarDoesntExistException(string message)
            : base(message) { }

        public UserWithCarDoesntExistException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
