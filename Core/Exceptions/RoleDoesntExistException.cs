using System.Net;

namespace Core.Exceptions
{
    public class RoleDoesntExistException : BaseException
    {
        public string RoleName { get; }
        public override string Message
        {
            get { return "Role " + RoleName + " does not exist."; }
        }

        public RoleDoesntExistException() { }

        public RoleDoesntExistException(string message)
            : base(message) { 
                RoleName = message;
            }

        public RoleDoesntExistException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
