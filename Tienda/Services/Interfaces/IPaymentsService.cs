using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Servicios.Interfaces
{
    public interface IPaymentsService
    {
        Task<int> CreatePayment(Payment pay);
        Task<int> UpdatePayment(Payment pay);
    }
}
