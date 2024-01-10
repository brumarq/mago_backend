using System.Net;

namespace Application.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException() : base(HttpStatusCode.Unauthorized)
    {
    }
    
    public UnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized)
    {
    }
}