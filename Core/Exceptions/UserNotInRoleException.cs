using System.Net;

namespace Core.Exceptions
{
    public class UserNotInRoleException : BaseException
    {
        public override string Message
        {
            get { return "User not in correct role."; }
        }

        public UserNotInRoleException() { }

        public UserNotInRoleException(string message)
            : base(message) { }

        public UserNotInRoleException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
