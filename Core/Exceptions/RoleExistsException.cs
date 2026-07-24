using System.Net;

namespace Core.Exceptions
{
    public class RoleExistsException : BaseException
    {
        public string RoleName { get; }
        public override string Message
        {
            get { return "Role " + RoleName + " already exists."; }
        }

        public RoleExistsException() { }

        public RoleExistsException(string message)
            : base(message) { 
                RoleName = message;
            }

        public RoleExistsException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}
