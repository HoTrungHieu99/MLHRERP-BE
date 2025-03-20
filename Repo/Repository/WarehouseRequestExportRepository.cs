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
    public class WarehouseRequestExportRepository : IWarehouseRequestExportRepository
    {
        private readonly MinhLongDbContext _context;

        public WarehouseRequestExportRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<RequestExport> GetRequestExportByIdAsync(int requestExportId)
        {
            return await _context.RequestExports
                .Include(re => re.RequestExportDetails)
                .FirstOrDefaultAsync(re => re.RequestExportId == requestExportId);
        }

        public async Task<WarehouseRequestExport> CreateWarehouseRequestExportAsync(long warehouseId, int requestExportId)
        {
            var requestExport = await GetRequestExportByIdAsync(requestExportId);
            if (requestExport == null || requestExport.RequestExportDetails == null || !requestExport.RequestExportDetails.Any())
            {
                return null;
            }

            var warehouseRequests = requestExport.RequestExportDetails.Select(detail => new WarehouseRequestExport
            {
                WarehouseId = warehouseId,
                RequestExportId = requestExportId,
                ProductId = detail.ProductId,
                QuantityRequested = detail.RequestedQuantity,
                RemainingQuantity = detail.RequestedQuantity,
                Status = "PENDING"
            }).ToList();

            await _context.WarehouseRequestExports.AddRangeAsync(warehouseRequests);
            await _context.SaveChangesAsync();

            return warehouseRequests.FirstOrDefault();
        }

        public async Task<WarehouseRequestExport> GetByIdAsync(int id)
        {
            return await _context.WarehouseRequestExports
                .Include(x => x.Product)
                .Include(x => x.RequestExport)
                .FirstOrDefaultAsync(x => x.WarehouseRequestExportId == id);
        }

        public async Task UpdateAsync(WarehouseRequestExport request)
        {
            _context.WarehouseRequestExports.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WarehouseRequestExport>> GetByWarehouseAndRequestExportAsync(int warehouseId, int requestExportId)
        {
            return await _context.WarehouseRequestExports
                .Where(x => x.WarehouseId == warehouseId && x.RequestExportId == requestExportId)
                .ToListAsync();
        }

        public async Task UpdateManyAsync(List<WarehouseRequestExport> requests)
        {
            _context.WarehouseRequestExports.UpdateRange(requests);
            await _context.SaveChangesAsync();
        }

        public async Task CreateNewRequestForOtherWarehouseAsync(int warehouseId, int requestExportId, Dictionary<long, int> remainingProducts)
        {
            var newRequestExport = new RequestExport
            {
                Status = "PENDING"
            };

            _context.RequestExports.Add(newRequestExport);
            await _context.SaveChangesAsync();

            foreach (var item in remainingProducts)
            {
                var newRequest = new WarehouseRequestExport
                {
                    RequestExportId = newRequestExport.RequestExportId,
                    WarehouseId = warehouseId, // Kho khác sẽ được chọn bởi hệ thống
                    ProductId = item.Key,
                    QuantityRequested = item.Value,
                    Status = "PENDING"
                };
                _context.WarehouseRequestExports.Add(newRequest);
            }

            await _context.SaveChangesAsync();
        }
    }
}
