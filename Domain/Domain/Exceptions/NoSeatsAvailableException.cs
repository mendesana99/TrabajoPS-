using System;

namespace Domain.Exceptions
{
    public class NoSeatsAvailableException : Exception
    {
        public NoSeatsAvailableException() : base("No hay butacas disponibles para esta selección.")
        {
        }

        public NoSeatsAvailableException(string message) : base(message)
        {
        }
    }
}
