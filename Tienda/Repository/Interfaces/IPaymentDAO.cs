using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Repository.Daos
{
    public interface IPaymentDAO
    {
        Task<Payment> GetPayment(int id);
        Task<int> CreatePayment(Payment pay);
        Task<int> UpdatePayment(Payment pay);
    }
}
