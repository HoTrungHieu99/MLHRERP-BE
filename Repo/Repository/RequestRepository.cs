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
            return await _context.RequestProducts.Include(r => r.AgencyAccount).Include(r => r.Product).ToListAsync();
        }

        public async Task<RequestProduct> GetRequestByIdAsync(long requestId)
        {
            return await _context.RequestProducts.Include(r => r.AgencyAccount).Include(r => r.Product)
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

        public async Task<bool> ApproveRequestAsync(long requestId, Guid userId) // 🔥 Sửa long thành Guid
        {
            var request = await _context.RequestProducts.FindAsync(requestId);
            if (request == null || request.RequestStatus != "PENDING")
                return false;

            // 🔥 Tìm EmployeeId dựa trên UserId
            var employeeId = await GetEmployeeIdByUserIdAsync(userId);
            if (employeeId == null)
                return false;

            request.ApprovedBy = employeeId.Value;
            request.RequestStatus = "APPROVED";

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                SalesAgentId = request.AgencyId,
                Discount = 0,
                FinalPrice = 0,
                Status = "PENDING",
                RequestId = request.RequestProductId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<long?> GetAgencyIdByUserIdAsync(Guid userId)
        {
            var agency = await _context.AgencyAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
            return agency?.AgencyId;
        }

        public async Task<long?> GetEmployeeIdByUserIdAsync(Guid userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            return employee?.EmployeeId;
        }
    }
}
