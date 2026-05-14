using System;

namespace Domain.Exceptions
{
    public class ConcurrencyException : DomainException
    {
        public ConcurrencyException(string message) : base(message, 409) // Conflict
        {
        }
    }
}
