using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Models;

namespace Tienda.Repository
{
    public interface IOrdersDAO
    {
        public Task<int> CreateOrder(Order order);
        public Task<Order> GetOrder(int? id);
        public Task<IEnumerable<Order>> GetOrders();
        public Task<int> UpdateOrder(Order order);
        public Task<int> CreateOrderDetail(OrderDetail orderDetails);
    }
}
