using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum AuditAction
    {
        RESERVE_ATTEMPT,
        RESERVE_SUCCESS,
        RESERVE_FAILED_CONCURRENCY,
        PAYMENT_SUCCESS,
        PAYMENT_FAILED,
        EXPIRED,
        RELEASE_SEAT
    }
}
