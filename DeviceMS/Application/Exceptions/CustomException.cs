using System.Net;

namespace Application.Exceptions;

public abstract class CustomException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected CustomException(HttpStatusCode statusCode) : this("", statusCode)
    {
        StatusCode = statusCode;
    }

    public CustomException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}