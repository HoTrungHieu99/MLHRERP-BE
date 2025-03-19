using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
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

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<RequestProduct> GetRequestProductByOrderAsync(Guid orderId)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null) return null;

            return await _context.RequestProducts
                .FirstOrDefaultAsync(rp => rp.RequestProductId == order.RequestId);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalQuantityByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderDetails
                                 .Where(od => od.OrderId == orderId)
                                 .SumAsync(od => od.Quantity);
        }

        public async Task<List<Order>> GetOrdersByAgencyIdAsync(long agencyId)
        {
            return await _context.Orders
                .Include(o => o.RequestProduct)
                .Where(o => o.RequestProduct.AgencyId == agencyId)
                .ToListAsync();
        }
    }
}
