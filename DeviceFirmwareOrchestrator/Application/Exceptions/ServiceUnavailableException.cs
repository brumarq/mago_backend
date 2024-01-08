using System.Net;

namespace Application.Exceptions;

public class ServiceUnavailableException : CustomException
{
    public ServiceUnavailableException() : base(HttpStatusCode.ServiceUnavailable)
    {
    }
    
    public ServiceUnavailableException(string message) : base(message, HttpStatusCode.ServiceUnavailable)
    {
    }
}