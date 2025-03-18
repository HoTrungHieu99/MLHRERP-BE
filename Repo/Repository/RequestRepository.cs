using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;

namespace Repo.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly MinhLongDbContext _context;

        public RequestRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequestProduct>> GetAllRequestsAsync()
        {
            return await _context.RequestProducts
                .Include(r => r.AgencyAccount)
                .Include(r => r.RequestProductDetails)
                .ThenInclude(d => d.Product)
                .ToListAsync();
        }

        public async Task<RequestProduct> GetRequestByIdAsync(long requestId)
        {
            return await _context.RequestProducts
                .Include(r => r.AgencyAccount)
                .Include(r => r.RequestProductDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(r => r.RequestProductId == requestId);
        }

        public async Task<RequestProduct> CreateRequestAsync(RequestProduct request)
           {
    _context.RequestProducts.Add(request);
    await _context.SaveChangesAsync();
    return request;
            }

        public async Task<RequestProduct> UpdateRequestAsync(RequestProduct request)
        {
            _context.RequestProducts.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> ApproveRequestAsync(long requestId, Guid userId)
        {
            var request = await _context.RequestProducts.FindAsync(requestId);
            if (request == null || request.RequestStatus != "PENDING")
                return false;

            var employeeId = await GetEmployeeIdByUserIdAsync(userId);
            if (employeeId == null)
                return false;

            var agencyId = await GetAgencyIdByUserIdAsync(userId);
            if (agencyId == null)
                return false;

            request.ApprovedBy = employeeId.Value;
            request.RequestStatus = "APPROVED";

            var orderId = Guid.NewGuid();

            var order = new Order
            {
                OrderId = orderId,
                OrderDate = DateTime.UtcNow,
                SalesAgentId = agencyId.Value,
                Discount = 0,
                FinalPrice = 0,
                Status = "PENDING",
                RequestId = request.RequestProductId
            };

            _context.Orders.Add(order);

            // 🔥 Lấy danh sách RequestProductDetail theo requestId
            var requestDetails = await _context.RequestProductDetails
                .Where(d => d.RequestProductId == requestId)
                .ToListAsync();

            var orderDetails = requestDetails.Select(detail => new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = 0,         // ✅ Đặt mặc định bằng 0
                TotalAmount = 0,       // ✅ Đặt mặc định bằng 0
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<long?> GetAgencyIdByUserIdAsync(Guid userId)
        {
            var agency = await _context.AgencyAccounts
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return agency?.AgencyId;
        }

        public async Task<long?> GetEmployeeIdByUserIdAsync(Guid userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            return employee?.EmployeeId;
        }

    }

}
