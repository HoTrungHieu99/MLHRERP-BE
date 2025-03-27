using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class WarehouseRequestExportService : IWarehouseRequestExportService
    {
        private readonly IWarehouseRequestExportRepository _repository;
        private readonly MinhLongDbContext _context;

        public WarehouseRequestExportService(IWarehouseRequestExportRepository repository, MinhLongDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<WarehouseRequestExport> CreateWarehouseRequestExportAsync(long warehouseId, int requestExportId)
        {
            return await _repository.CreateWarehouseRequestExportAsync(warehouseId, requestExportId);
        }

        public async Task<List<WarehouseRequestExportDtoResponse>> GetByWarehouseIdAsync(long warehouseId)
        {
            var data = await _repository.GetByWarehouseIdAsync(warehouseId);

            return data.Select(x => new WarehouseRequestExportDtoResponse
            {
                WarehouseRequestExportId = x.WarehouseRequestExportId,
                RequestExportId = x.RequestExportId,
                ProductId = x.ProductId,
                ProductName = x.Product.ProductName,
                AgencyName = x.RequestExport.Order.RequestProduct.AgencyAccount.AgencyName,
                OrderCode = x.RequestExport.Order.OrderCode,
                QuantityRequested = x.QuantityRequested,
                RemainingQuantity = x.RemainingQuantity,
                Status = x.Status,
            }).ToList();
        }

        public async Task<bool> ApproveRequestAsync(int warehouseRequestExportId, int quantityApproved, Guid approvedBy)
        {
            var request = await _repository.GetByIdAsync(warehouseRequestExportId);
            if (request == null)
                throw new KeyNotFoundException("Warehouse request not found");

            if (request.QuantityRequested < quantityApproved)
                throw new InvalidOperationException("Approved quantity cannot exceed requested quantity");

            request.QuantityApproved = quantityApproved;
            request.ApprovedBy = approvedBy;
            request.Status = "APPROVED";
            request.RemainingQuantity = request.QuantityRequested - quantityApproved;

            await _repository.UpdateAsync(request);

            var relatedRequests = await _context.WarehouseRequestExports
        .Where(x => x.RequestExportId == request.RequestExportId)
        .ToListAsync();

            bool allCompleted = relatedRequests.All(x => x.RemainingQuantity == 0);

            if (allCompleted)
            {
                var requestExport = await _context.RequestExports
                    .FirstOrDefaultAsync(x => x.RequestExportId == request.RequestExportId);

                if (requestExport != null)
                {
                    requestExport.Status = "Approved";
                    _context.RequestExports.Update(requestExport);
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> ApproveAllRequestsByWarehouseAsync(int warehouseId, int requestExportId, Dictionary<int, int> quantitiesApproved, Guid approvedBy)
        {
            var requests = await _repository.GetByWarehouseAndRequestExportAsync(warehouseId, requestExportId);
            if (requests == null || !requests.Any())
                throw new KeyNotFoundException("No warehouse requests found for this WarehouseId and RequestExportId");

            foreach (var request in requests)
            {
                if (!quantitiesApproved.ContainsKey(request.WarehouseRequestExportId))
                    continue; // Nếu không có số lượng duyệt, bỏ qua

                int approvedQuantity = quantitiesApproved[request.WarehouseRequestExportId];

                if (approvedQuantity > request.QuantityRequested)
                    throw new InvalidOperationException($"Approved quantity for product {request.ProductId} exceeds requested quantity");

                request.QuantityApproved = approvedQuantity;
                request.ApprovedBy = approvedBy;
                request.Status = "APPROVED";
                request.RemainingQuantity = request.QuantityRequested - approvedQuantity;
            }

            await _repository.UpdateManyAsync(requests);
            return true;
        }
    }
}
