using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid orderId);
        //Task<bool> ProcessPaymentAsync(Guid orderId);
        Task<bool> CancelOrderAsync(Guid orderId);

        Task<List<Order>> GetOrdersByAgencyIdAsync(long agencyId);

        Task<bool> ProcessPaymentAsync(Guid orderId);
    }
}
