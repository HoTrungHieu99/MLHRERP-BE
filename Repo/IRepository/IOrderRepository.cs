using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();

        Task AddOrderDetailAsync(List<OrderDetail> orderDetails);
    }
}
