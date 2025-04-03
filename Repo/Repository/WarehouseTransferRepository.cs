﻿using System;
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
    public class WarehouseTransferRepository : IWarehouseTransferRepository
    {
        private readonly MinhLongDbContext _context;

        public WarehouseTransferRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<WarehouseTransferRequest> CreateAsync(WarehouseTransferRequest request)
        {
            _context.WarehouseTransferRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<WarehouseTransferRequest>> GetAllAsync()
        {
            return await _context.WarehouseTransferRequests
                .Include(r => r.TransferProducts)
                .ToListAsync();
        }

        public async Task<WarehouseTransferRequest?> GetByIdAsync(long id)
        {
            return await _context.WarehouseTransferRequests
                .Include(r => r.TransferProducts)
                .Include(r => r.RequestExport)
                    .ThenInclude(re => re.Order) // nếu RequestExport liên kết đến Order
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> PlanTransferRequestAsync(long requestId, long sourceWarehouseId, Guid plannerId)
        {
            var request = await _context.WarehouseTransferRequests.FindAsync(requestId);
            if (request == null) return false;

            request.SourceWarehouseId = sourceWarehouseId;
            request.PlannedBy = plannerId;
            request.Status = "Planned";

            _context.WarehouseTransferRequests.Update(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<WarehouseTransferRequest>> GetPlannedRequestsByWarehouseAsync(long sourceWarehouseId)
        {
            return await _context.WarehouseTransferRequests
                .Include(r => r.TransferProducts)
                .Where(r => r.SourceWarehouseId == sourceWarehouseId && r.Status == "Planned")
                .ToListAsync();
        }

        public async Task<RequestExport?> GetRequestExportWithOrderAsync(int requestExportId)
        {
            return await _context.RequestExports
                .Include(re => re.Order)
                .FirstOrDefaultAsync(re => re.RequestExportId == requestExportId);
        }

        public async Task<WarehouseTransferRequest?> GetByIdWithProductsAsync(long id)
        {
            return await _context.WarehouseTransferRequests
                .Include(x => x.TransferProducts)
                .Include(x => x.ExportWarehouseReceipts.Where(e => e.Status == "Approved"))
                    .ThenInclude(e => e.ExportWarehouseReceiptDetails)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(WarehouseTransferRequest request)
        {
            _context.WarehouseTransferRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
