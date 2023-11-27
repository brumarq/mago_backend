using System.Net;

namespace Application.Exceptions
{
    public abstract class CustomException : Exception
    {
        private readonly HttpStatusCode _statusCode;

        protected CustomException(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public CustomException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            _statusCode = statusCode;
        }

    }
}
