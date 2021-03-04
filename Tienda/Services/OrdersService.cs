using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;
using Tienda.Repository;
using Tienda.Servicios.Interfaces;
using P2P = PlacetoPay.Integrations.Library.CSharp.PlacetoPay;
using PlacetoPay.Integrations.Library.CSharp.Contracts;
using PlacetoPay.Integrations.Library.CSharp.Entities;
using PlacetoPay.Integrations.Library.CSharp.Message;
using Tienda.Repository.Daos;

namespace Tienda.Servicios
{
    public class OrdersService: IOrdersService
    {
        #region Atributos
        private IOrdersDAO _ordersDao;
        private IPaymentsService _paymentService;
        private const string urlP2P = "https://test.placetopay.com/redirection/";

        #endregion

        #region Constructor

        public OrdersService(IOrdersDAO ordersDAO, IPaymentsService paymentService)
        {
            this._ordersDao = ordersDAO;
            this._paymentService = paymentService;
        }

        public async Task<int> CreateOrderDetails(OrderDetail orderDetails)
        {
            return await this._ordersDao.CreateOrderDetail(orderDetails);
        }

        public async Task<PaymentStateResponse> GetOrderDetails(int? id)
        {
            var order = await _ordersDao.GetOrder(id);

            PaymentStateResponse paymentStateResponse = new PaymentStateResponse();
            paymentStateResponse.IsRejected = false;
            paymentStateResponse.Id = order.Id;
            paymentStateResponse.CustomerName = order.CustomerName;
            paymentStateResponse.CreatedAt = order.CreatedAt;
            paymentStateResponse.CustomerDocument = order.CustomerDocument;
            paymentStateResponse.CustomerEmail = order.CustomerEmail;
            paymentStateResponse.OrderDetails = order.OrderDetails;
            paymentStateResponse.Payments = order.Payments;
            paymentStateResponse.Status = order.Status;
            paymentStateResponse.UpdatedAt = order.UpdatedAt;
            paymentStateResponse.ValorOrder = order.ValorOrder;

            if (order.Payments.Count > 0)
            {               
                var pay = orderHasDeclinedPayments(order);

                if (pay != null)
                {
                    if (pay.Status != "PAYED")
                    {
                        Gateway gateway = GetGateway();
                        RedirectInformation response = gateway.Query(pay.RequestId.ToString());

                        if (response.IsSuccessful())
                        {
                            VerifyPaymentState(response,order,pay);
                            if(pay.Status == "REJECTED")
                            {
                                paymentStateResponse.IsRejected = true;
                            }                          
                        }
                        else
                        {
                            paymentStateResponse.Messagge = response.Status.Message;
                        }
                        await _paymentService.UpdatePayment(pay);
                        await _ordersDao.UpdateOrder(order);
                    }
                }
                else
                {
                    // Tiene pagos rechazados
                    paymentStateResponse.IsRejected = true;
                }
            }
            return paymentStateResponse;
        }

        public async Task<Order> GetOrder(int id)
        {
            return await _ordersDao.GetOrder(id);
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _ordersDao.GetOrders();
        }

        public async Task<int> CreateOrder(Order order)
        {
            return await _ordersDao.CreateOrder(order);
        }
        #endregion

        #region Métodos

        private Models.Payment orderHasDeclinedPayments(Order order)
        {
            var pay = (from p in order.Payments
                       where p.Status != "REJECTED"
                       select p).FirstOrDefault();

            return pay;
        }

        public Gateway GetGateway()
        {
            
            return new P2P(Environment.GetEnvironmentVariable("Login"),
                            Environment.GetEnvironmentVariable("TranKey") == null ? "024h1IlD" 
                            : Environment.GetEnvironmentVariable("TranKey"),
                            new Uri(Environment.GetEnvironmentVariable("UrlBase") == null 
                            ? urlP2P : Environment.GetEnvironmentVariable("UrlBase")), Gateway.TP_REST);

        }

        private void VerifyPaymentState(RedirectInformation response, Order order, Models.Payment pay)
        {
            switch (response.Status.status)
            {
                case "APPROVED":
                    order.Status = "PAYED";
                    order.UpdatedAt = Convert.ToDateTime(response.Status.Date);
                    pay.Status = response.Status.status;
                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                    break;

                case "PENDING":
                    pay.Status = response.Status.status;
                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                    break;

                case "REJECTED":
                    pay.Status = response.Status.status;
                    pay.FechaUpdate = Convert.ToDateTime(response.Status.Date);
                    pay.Message = response.Status.Message;
                    break;
            }
        }
        #endregion

    }
}
