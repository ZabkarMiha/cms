using System.Net;

namespace Core.Exceptions
{
    public class DoesNotManageException : BaseException
    {
        public string ManagerName { get; }
        public override string Message
        {
            get { return ManagerName + " doesn't manage this user or car"; }
        }

        public DoesNotManageException() { }

        public DoesNotManageException(string message)
            : base(message) { 
                ManagerName = message;
            }

        public DoesNotManageException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotAcceptable;
    }
}