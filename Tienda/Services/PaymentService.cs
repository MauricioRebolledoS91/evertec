using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;
using Tienda.Repository.Daos;
using Tienda.Servicios.Interfaces;

namespace Tienda.Servicios
{
    public class PaymentService: IPaymentsService
    {
        #region Atributos

        private IPaymentDAO _paymentDAO;

        #endregion

        #region Constructor

        public PaymentService(IPaymentDAO paymentDAO)
        {
            this._paymentDAO = paymentDAO;
        }

        #endregion

        #region Métodos

        // RETORNA 0 SI NO SE HA EJECUTADO LA ACCIÓN O SI HA HABIDO UN ERROR
        public async Task<int> CreatePayment(Payment pay)
        {
            var _result = await _paymentDAO.CreatePayment(pay);
            return _result;
        }

        public async Task<int> UpdatePayment(Payment pay)
        {
            var _result = await _paymentDAO.UpdatePayment(pay);
            return _result;
        }

        #endregion
    }
}
