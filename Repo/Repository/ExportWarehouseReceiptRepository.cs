using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class ExportWarehouseReceiptRepository : IExportWarehouseReceiptRepository
    {
        private readonly MinhLongDbContext _context;
        public ExportWarehouseReceiptRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<List<WarehouseProduct>> GetWarehouseProductsByIdsAsync(List<long> warehouseProductIds)
        {
            return await _context.WarehouseProduct
                .Include(p => p.Product)  // 🔥 Đảm bảo lấy Product
                .Include(p => p.Batch)    // 🔥 Đảm bảo lấy Batch (BatchNumber)
                .Where(p => warehouseProductIds.Contains(p.WarehouseProductId))
                .ToListAsync();
        }

        public async Task<ExportWarehouseReceipt> CreateReceiptAsync(ExportWarehouseReceipt receipt)
        {
            _context.ExportWarehouseReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task<ExportWarehouseReceipt> GetReceiptByIdAsync(long id)
        {
            return await _context.ExportWarehouseReceipts
                .Include(r => r.ExportWarehouseReceiptDetails)
                .Include(r => r.RequestExport)
                    .ThenInclude(req => req.Order)
                        .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(x => x.AgencyAccount)
                .FirstOrDefaultAsync(r => r.ExportWarehouseReceiptId == id);
        }

        public async Task ApproveReceiptAsync(long id)
        {
            var receipt = await _context.ExportWarehouseReceipts
                .Include(r => r.ExportWarehouseReceiptDetails)
                .Include(r => r.RequestExport)
                    .ThenInclude(req => req.Order)
                        .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(x => x.AgencyAccount)
                .FirstOrDefaultAsync(r => r.ExportWarehouseReceiptId == id);

            if (receipt == null) throw new Exception("Receipt not found");
            receipt.Status = "Approved";

            var exportTransaction = new ExportTransaction
            {
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                ExportDate = receipt.ExportDate,
                ExportType = receipt.ExportType,
                WarehouseId = receipt.WarehouseId,
                Note = "Approved Export",
                RequestExportId = receipt.RequestExportId,  // 🔥 Thêm RequestExportId
                OrderCode = receipt.RequestExport.Order.OrderCode,  // 🔥 Lấy OrderCode
                AgencyName = receipt.RequestExport.Order.RequestProduct.AgencyAccount.AgencyName,  // 🔥 Lấy AgencyName
                ExportTransactionDetail = receipt.ExportWarehouseReceiptDetails.Select(d => new ExportTransactionDetail
                {
                    WarehouseProductId = d.WarehouseProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    TotalProductAmount = d.TotalProductAmount,
                    ExpiryDate = d.ExpiryDate
                }).ToList()
            };

            _context.ExportTransactions.Add(exportTransaction);
            await _context.SaveChangesAsync();
        }
    }

}
