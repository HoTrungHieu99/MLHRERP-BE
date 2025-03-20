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
