using System.Net;

namespace Application.Exceptions
{
    public class ForbiddenException : CustomException
    {
        public ForbiddenException() : base(HttpStatusCode.Forbidden)
        {
        }

        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}
