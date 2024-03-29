﻿using System.Net;

namespace Application.Exceptions
{
    public abstract class CustomException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        protected CustomException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public CustomException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

    }
}
