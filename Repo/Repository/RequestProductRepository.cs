using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
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
                        .Include(r => r.AgencyAccount) // 👈 Thêm dòng này
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
            .Include(rp => rp.AgencyAccount) // ✅ Bao gồm thông tin đại lý
            .Include(rp => rp.ApprovedByEmployee) // ✅ Bao gồm nhân viên duyệt (nếu có)
            .Include(rp => rp.RequestProductDetails) // ✅ Bao gồm danh sách sản phẩm trong request
                .ThenInclude(rpd => rpd.Product) // ✅ Bao gồm thông tin sản phẩm
            .FirstOrDefaultAsync(rp => rp.RequestProductId == id);
        }

        public async Task AddRequestAsync(RequestProduct requestProduct)
        {
            await _context.RequestProducts.AddAsync(requestProduct);
        }

        public async Task UpdateRequestAsync(RequestProduct requestProduct)
        {
            requestProduct.UpdatedAt = DateTime.UtcNow; // ✅ Gán thời gian hiện tại trước khi update
            _context.RequestProducts.Update(requestProduct);
            //_context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<RequestProduct> GetRequestProductByRequestIdAsync(Guid requestId)
        {
            return await _context.RequestProducts
             .Where(rq => rq.RequestProductId == requestId)
            .Include(rp => rp.RequestProductDetails) // ✅ Bao gồm các sản phẩm trong Request
            .FirstOrDefaultAsync(rp => rp.RequestProductId == requestId);
        }

        public async Task<List<RequestProduct>> GetRequestProductAgencyIdAsync(long agencyId)
        {
            return await _context.RequestProducts
                .Where(rp => rp.AgencyId == agencyId) // ✅ Lọc theo AgencyId
                .Include(rp => rp.RequestProductDetails) // ✅ Bao gồm danh sách sản phẩm
                .ThenInclude(d => d.Product) // ✅ Bao gồm thông tin sản phẩm
                .ToListAsync();
        }


        public async Task<List<RequestProduct>> GetRequestProductByIdAsync(Guid requestId)
        {
            return await _context.RequestProducts
                        .Where(rp => rp.RequestProductId == requestId)
                        .Include(rp => rp.RequestProductDetails)
                        .ThenInclude(d => d.Product)
                        .Include(rp => rp.AgencyAccount) // ✅ Thêm để lấy AgencyName
                        .ToListAsync();
        }

        public async Task<string> GenerateRequestCodeAsync()
        {
            var today = DateTime.Now.Date;
            int countToday = await _context.RequestProducts
                .Where(r => r.CreatedAt.Date == today)
                .CountAsync();

            string datePart = today.ToString("yyyyMMdd");
            string requestCode = $"RQ{datePart}-{(countToday + 1):D3}";

            return requestCode;
        }

        public async Task<string> GenerateOrderCodeAsync()
        {
            var today = DateTime.Now.Date;
            int countToday = await _context.RequestProducts
                .Where(r => r.CreatedAt.Date == today)
                .CountAsync();

            string datePart = today.ToString("yyyyMMdd");
            string requestCode = $"ORD{datePart}-{(countToday + 1):D3}";

            return requestCode;
        }


    }

}
