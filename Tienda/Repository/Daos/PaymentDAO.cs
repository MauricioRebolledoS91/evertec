using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Repository.Daos
{
    public class PaymentDAO: IPaymentDAO
    {
        #region Atributos

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructor

        public PaymentDAO(ApplicationDbContext context)
        {
            this._context = context;
        }

        #endregion

        //#region Métodos

        public async Task<IEnumerable<Payment>> GetPayments()
        {
            return await this._context.Payments.ToListAsync();
        }

        public async Task<Payment> GetPayment(int id)
        {
            return await this._context.Payments.SingleOrDefaultAsync(m => m.PaymentId == id);
        }

        public async Task<int> CreatePayment(Payment pay)
        {
            var _result = 0;
            this._context.Payments.Add(pay);
            try
            {
                _result = await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _result = 0;
            }
            catch (DbUpdateException)
            {
                _result = 0;
            }

            return _result;
        }

        public async Task<int> UpdatePayment(Payment pay)
        {
            var _result = 0;
            this._context.Entry(pay).State = EntityState.Modified;
            try
            {
                _result = await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _result = 0;
            }
            catch (DbUpdateException ex)
            {
                _result = 0;
            }

            if (!await PagoExists(pay.PaymentId))
            {
                _result = 0;
            }

            return _result;
        }

        private async Task<bool> PagoExists(int id)
        {
            return await this._context.Payments.AnyAsync(e => e.PaymentId == id);
        }

    }
}
