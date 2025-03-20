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
    }
}
