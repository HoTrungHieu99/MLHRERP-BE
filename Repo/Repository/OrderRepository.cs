using BusinessObject.Models;
using DataAccessLayer;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MinhLongDbContext _context;

        public OrderRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddOrderDetailAsync(List<OrderDetail> orderDetails) // ✅ Thêm phương thức này
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails); // 🔹 Thêm danh sách `OrderDetail` cùng lúc
        }

    }
}
