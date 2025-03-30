using BusinessObject.DTO.Warehouse;
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

        public async Task<ExportWarehouseReceipt> CreateFromRequestAsync(int requestExportId, long warehouseId)
        {
            var requestExport = await _context.RequestExports
                .Include(x => x.RequestExportDetails)
                .Include(x => x.Order)
                    .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(x => x.AgencyAccount)
                .FirstOrDefaultAsync(x => x.RequestExportId == requestExportId);

            if (requestExport == null)
                throw new KeyNotFoundException("RequestExport not found");

            if (requestExport.RequestExportDetails == null || !requestExport.RequestExportDetails.Any())
                throw new InvalidOperationException("RequestExport has no details");

            var receipt = new ExportWarehouseReceipt
            {
                DocumentNumber = $"EXP-{DateTime.Now:yyyyMMddHHmmss}",
                DocumentDate = DateTime.Now,
                ExportDate = DateTime.Now,
                ExportType = "Xuất Bán",
                TotalQuantity = requestExport.RequestExportDetails.Sum(d => d.RequestedQuantity),
                TotalAmount = 0,
                RequestExportId = requestExportId,
                AgencyName = requestExport.Order?.RequestProduct?.AgencyAccount?.AgencyName,
                OrderCode = requestExport.Order?.OrderCode,
                WarehouseId = warehouseId,
                Status = "Pending",
                ExportWarehouseReceiptDetails = new List<ExportWarehouseReceiptDetail>()
            };

            decimal totalAmount = 0;

            foreach (var detail in requestExport.RequestExportDetails)
            {
                int quantityNeeded = detail.RequestedQuantity;

                // 🔍 Lấy danh sách warehouseProduct theo ProductId và WarehouseId (có tồn kho > 0), ưu tiên FIFO
                var availableProducts = await _context.WarehouseProduct
                    .Include(wp => wp.Product)
                    .Include(wp => wp.Batch)
                    .Where(wp => wp.ProductId == detail.ProductId
                                 && wp.WarehouseId == warehouseId
                                 && wp.Quantity > 0)
                    .OrderBy(wp => wp.Batch.ExpiryDate) // FIFO: Xuất hàng gần hết hạn trước
                    .ToListAsync();

                if (availableProducts == null || !availableProducts.Any())
                    throw new InvalidOperationException($"No available WarehouseProduct for ProductId = {detail.ProductId} in WarehouseId = {warehouseId}");

                foreach (var wp in availableProducts)
                {
                    if (quantityNeeded <= 0)
                        break;

                    int quantityToExport = Math.Min(quantityNeeded, wp.Quantity);
                    var unitPrice = wp.Batch.SellingPrice ?? 0;
                    var amount = quantityToExport * unitPrice;

                    receipt.ExportWarehouseReceiptDetails.Add(new ExportWarehouseReceiptDetail
                    {
                        WarehouseProductId = wp.WarehouseProductId,
                        ProductId = detail.ProductId,
                        ProductName = wp.Product.ProductName,
                        BatchNumber = wp.Batch.BatchCode,
                        Quantity = quantityToExport,
                        UnitPrice = unitPrice,
                        TotalProductAmount = amount,
                        ExpiryDate = wp.Batch.ExpiryDate
                    });

                    totalAmount += amount;
                    quantityNeeded -= quantityToExport;
                }

                if (quantityNeeded > 0)
                    throw new InvalidOperationException($"Not enough stock for ProductId = {detail.ProductId}. Still missing: {quantityNeeded}");
            }

            receipt.TotalAmount = totalAmount;

            await _context.ExportWarehouseReceipts.AddAsync(receipt);
            await _context.SaveChangesAsync();

            return receipt;
        }

        public async Task<bool> UpdateFullAsync(UpdateExportWarehouseReceiptFullDto dto)
        {
            var receipt = await _context.ExportWarehouseReceipts
                .Include(r => r.ExportWarehouseReceiptDetails)
                .FirstOrDefaultAsync(r => r.ExportWarehouseReceiptId == dto.ExportWarehouseReceiptId);

            if (receipt == null)
                throw new KeyNotFoundException("ExportWarehouseReceipt not found.");

            if (receipt.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot update a receipt that has already been approved.");

            // Cập nhật thông tin chung
            receipt.ExportDate = dto.ExportDate;
            receipt.ExportType = dto.ExportType;

            // Xoá chi tiết
            if (dto.DeleteDetailIds != null && dto.DeleteDetailIds.Any())
            {
                var detailsToRemove = receipt.ExportWarehouseReceiptDetails
                    .Where(d => dto.DeleteDetailIds.Contains(d.ExportWarehouseReceiptDetailId))
                    .ToList();

                _context.ExportWarehouseReceiptDetail.RemoveRange(detailsToRemove);
            }

            // Cập nhật chi tiết (chỉ quantity, các trường khác lấy lại từ WarehouseProduct)
            foreach (var upd in dto.UpdateDetails)
            {
                var detail = receipt.ExportWarehouseReceiptDetails
                    .FirstOrDefault(d => d.ExportWarehouseReceiptDetailId == upd.ExportWarehouseReceiptDetailId);

                if (detail != null)
                {
                    var wp = await _context.WarehouseProduct
                        .Include(p => p.Product)
                        .Include(p => p.Batch)
                        .FirstOrDefaultAsync(p => p.WarehouseProductId == detail.WarehouseProductId);

                    if (wp == null)
                        throw new InvalidOperationException($"WarehouseProduct not found: {detail.WarehouseProductId}");

                    detail.Quantity = upd.Quantity;
                    detail.UnitPrice = wp.Batch.SellingPrice ?? 0;
                    detail.TotalProductAmount = detail.Quantity * detail.UnitPrice;
                    detail.ProductName = wp.Product.ProductName;
                    detail.BatchNumber = wp.Batch.BatchCode;
                    detail.ExpiryDate = wp.Batch.ExpiryDate;
                }
            }

            // Thêm mới chi tiết (tương tự như khi tạo)
            foreach (var add in dto.AddDetails)
            {
                var wp = await _context.WarehouseProduct
                    .Include(p => p.Product)
                    .Include(p => p.Batch)
                    .FirstOrDefaultAsync(p => p.WarehouseProductId == add.WarehouseProductId);

                if (wp == null)
                    throw new InvalidOperationException($"WarehouseProduct not found: {add.WarehouseProductId}");

                var unitPrice = wp.Batch.SellingPrice ?? 0;
                var total = unitPrice * add.Quantity;

                receipt.ExportWarehouseReceiptDetails.Add(new ExportWarehouseReceiptDetail
                {
                    WarehouseProductId = add.WarehouseProductId,
                    ProductId = add.ProductId,
                    ProductName = wp.Product.ProductName,
                    BatchNumber = wp.Batch.BatchCode,
                    Quantity = add.Quantity,
                    UnitPrice = unitPrice,
                    TotalProductAmount = total,
                    ExpiryDate = wp.Batch.ExpiryDate
                });
            }

            // Cập nhật tổng
            receipt.TotalQuantity = receipt.ExportWarehouseReceiptDetails.Sum(x => x.Quantity);
            receipt.TotalAmount = receipt.ExportWarehouseReceiptDetails.Sum(x => x.TotalProductAmount);

            _context.ExportWarehouseReceipts.Update(receipt);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
