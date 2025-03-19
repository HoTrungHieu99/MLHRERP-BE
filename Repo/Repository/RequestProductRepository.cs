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
    public class RequestProductRepository : IRequestProductRepository
    {
        private readonly MinhLongDbContext _context;

        public RequestProductRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequestProduct>> GetAllRequestsAsync()
        {
            return await _context.RequestProducts
                .Include(r => r.RequestProductDetails)
                .ThenInclude(d => d.Product)
                .ToListAsync();
        }

        // 🔹 Lấy đơn hàng Pending của Agency
        public async Task<RequestProduct> GetPendingRequestByAgencyAsync(long agencyId)
        {
            return await _context.RequestProducts
                .Include(r => r.RequestProductDetails)
                .FirstOrDefaultAsync(r => r.AgencyId == agencyId && r.RequestStatus == "Pending");
        }

        // 🔹 Kiểm tra nếu Agency có đơn hàng Approved trong 24 giờ qua
        public async Task<bool> HasApprovedRequestInLast24Hours(long agencyId)
        {
            return await _context.RequestProducts
            .AnyAsync(r => r.AgencyId == agencyId &&
                        r.RequestStatus == "Approved" &&
                        (r.UpdatedAt ?? r.CreatedAt) >= DateTime.UtcNow.AddHours(-24));
        }

        public async Task<RequestProduct> GetRequestByIdAsync(Guid id)
        {
            return await _context.RequestProducts
                .Include(r => r.RequestProductDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(r => r.RequestProductId == id);
        }

        public async Task AddRequestAsync(RequestProduct requestProduct)
        {
            await _context.RequestProducts.AddAsync(requestProduct);
        }

        public async Task UpdateRequestAsync(RequestProduct requestProduct)
        {
            requestProduct.UpdatedAt = DateTime.UtcNow; // ✅ Gán thời gian hiện tại trước khi update
            _context.RequestProducts.Update(requestProduct);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<RequestProduct> GetRequestProductByRequestIdAsync(Guid requestId)
        {
            return await _context.RequestProducts
            .Include(rp => rp.RequestProductDetails) // ✅ Bao gồm các sản phẩm trong Request
            .FirstOrDefaultAsync(rp => rp.RequestProductId == requestId);
        }
    }

}
