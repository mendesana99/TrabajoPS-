using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Exceptions.DomainExceptions;

namespace Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object key)
            : base($"No se encontró la entidad '{entityName}' con clave '{key}'.") { }
    }
}
