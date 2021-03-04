using PlacetoPay.Integrations.Library.CSharp.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Servicios.Interfaces
{
    public interface IOrdersService
    {
        Task<PaymentStateResponse> GetOrderDetails(int? id);
        Task<int> CreateOrder(Order order);
        Task<IEnumerable<Order>> GetOrders();
        Task<Order> GetOrder(int id);
        Task<int> CreateOrderDetails(OrderDetail orderDetails);
        Gateway GetGateway();
    }
}
