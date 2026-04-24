using System;

namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        public int StatusCode { get; }

        public DomainException(string message) : base(message)
        {
            StatusCode = 400; // Por defecto
        }

        public DomainException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
