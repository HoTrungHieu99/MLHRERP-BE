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

        public async Task<List<WarehouseRequestExport>> GetByWarehouseIdAsync(long warehouseId)
        {
            return await _context.WarehouseRequestExports
                .Include(x => x.Product)
                .Include(x => x.RequestExport)
                .ThenInclude(re => re.Order)
                .ThenInclude(o => o.RequestProduct)
                .ThenInclude(o => o.AgencyAccount)
                .Where(x => x.WarehouseId == warehouseId)
                .ToListAsync();
        }

        public async Task<List<WarehouseRequestExport>> GetByRequestExportIdAsync(int requestExportId)
        {
            return await _context.WarehouseRequestExports
                .Where(x => x.RequestExportId == requestExportId)
                .ToListAsync();
        }
    }
}
