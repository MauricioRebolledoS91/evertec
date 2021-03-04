using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tienda.Models
{
    public class PaymentStateResponse: Order
    {
        public PaymentStateResponse(): base()
        {

        }
        public string Messagge { get; set; }
        public bool IsRejected { get; set; }
    }
}
