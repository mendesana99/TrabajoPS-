using System;

namespace Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message, 404)
        {
        }
    }
}
