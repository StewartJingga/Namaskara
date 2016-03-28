using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public enum OrderStatusEnum
    {
        OrderSubmitted,
        WaitingForConfirmation,
        PaymentConfirmed,
        Shipped
    }
}