using System.Net;

namespace Application.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException() : base(HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
