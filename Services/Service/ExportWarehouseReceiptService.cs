using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ExportWarehouseReceiptService : IExportWarehouseReceiptService
    {
        private readonly IExportWarehouseReceiptRepository _repository;
        private readonly MinhLongDbContext _context;

        public ExportWarehouseReceiptService(IExportWarehouseReceiptRepository repository, MinhLongDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ExportWarehouseReceipt> CreateReceiptAsync(ExportWarehouseReceiptDTO dto)
        {
            var warehouseProducts = await _repository.GetWarehouseProductsByIdsAsync(
                dto.Details.Select(d => d.WarehouseProductId).ToList()
            );

            foreach (var detail in dto.Details)
            {
                var warehouseProduct = warehouseProducts.FirstOrDefault(p => p.WarehouseProductId == detail.WarehouseProductId);
                if (warehouseProduct == null)
                {
                    throw new Exception($"WarehouseProductId {detail.WarehouseProductId} not found in database.");
                }

                if (warehouseProduct.Batch == null || string.IsNullOrEmpty(warehouseProduct.Batch.BatchCode))
                {
                    throw new Exception($"BatchNumber is missing for WarehouseProductId {detail.WarehouseProductId}.");
                }

                if (warehouseProduct.Quantity < detail.Quantity)
                {
                    throw new Exception($"Insufficient quantity for WarehouseProductId {detail.WarehouseProductId}. Available: {warehouseProduct.Quantity}, Requested: {detail.Quantity}");
                }
            }

            var requestExport = await _context.RequestExports
        .Include(r => r.Order)
            .ThenInclude(o => o.RequestProduct)
            .ThenInclude(x => x.AgencyAccount)
        .FirstOrDefaultAsync(r => r.RequestExportId == dto.RequestExportId);

            if (requestExport == null)
            {
                throw new Exception($"RequestExportId {dto.RequestExportId} not found in database.");
            }

            var receiptDetails = dto.Details.Select(d =>
            {
                var warehouseProduct = warehouseProducts.First(p => p.WarehouseProductId == d.WarehouseProductId);
                return new ExportWarehouseReceiptDetail
                {
                    WarehouseProductId = d.WarehouseProductId,
                    ProductId = warehouseProduct.ProductId,
                    ProductName = warehouseProduct.Product.ProductName,
                    BatchNumber = warehouseProduct.Batch.BatchCode,
                    Quantity = d.Quantity,
                    UnitPrice = warehouseProduct.Batch.SellingPrice ?? 0,
                    TotalProductAmount = d.Quantity * (warehouseProduct.Batch.SellingPrice ?? 0),
                    ExpiryDate = warehouseProduct.ExpirationDate
                };
            }).ToList();

            var receipt = new ExportWarehouseReceipt
            {
                DocumentNumber = "EXP-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                DocumentDate = DateTime.Now,
                ExportDate = dto.ExportDate,
                ExportType = dto.ExportType,
                WarehouseId = dto.WarehouseId,
                Status = "Pending",
                TotalQuantity = receiptDetails.Sum(d => d.Quantity),
                TotalAmount = receiptDetails.Sum(d => d.TotalProductAmount),
                ExportWarehouseReceiptDetails = receiptDetails,
                RequestExportId = dto.RequestExportId,
                OrderCode = requestExport.Order.OrderCode, // 🔥 Lấy từ RequestExport
                AgencyName = requestExport.Order.RequestProduct.AgencyAccount.AgencyName // 🔥 Lấy từ SalesAgent
            };

            return await _repository.CreateReceiptAsync(receipt);
        }

        public async Task ApproveReceiptAsync(long id, Guid approvedBy)
        {
            var receipt = await _repository.GetReceiptByIdAsync(id);
            if (receipt == null) throw new Exception("Receipt not found");

            receipt.Status = "Approved";

            var warehouseProducts = await _repository.GetWarehouseProductsByIdsAsync(
                receipt.ExportWarehouseReceiptDetails.Select(d => d.WarehouseProductId).ToList()
            );

            foreach (var detail in receipt.ExportWarehouseReceiptDetails)
            {
                var warehouseProduct = warehouseProducts.First(wp => wp.WarehouseProductId == detail.WarehouseProductId);

                if (warehouseProduct.Quantity < detail.Quantity)
                    throw new Exception($"Insufficient quantity in warehouse for product {warehouseProduct.ProductId}");

                warehouseProduct.Quantity -= detail.Quantity;

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == warehouseProduct.ProductId);
                if (product != null)
                {
                    if (product.AvailableStock < detail.Quantity)
                        throw new Exception($"Product stock is insufficient for product {product.ProductId}");

                    product.AvailableStock -= detail.Quantity;
                    _context.Products.Update(product);
                }
            }

            // ✅ Gộp số lượng xuất theo ProductId
            var groupedExportDetails = receipt.ExportWarehouseReceiptDetails
                .GroupBy(d => d.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Quantity));

            // ✅ Cập nhật WarehouseRequestExport
            var requestExports = await _context.WarehouseRequestExports
                .Where(x => x.RequestExportId == receipt.RequestExportId && x.WarehouseId == receipt.WarehouseId)
                .ToListAsync();

            foreach (var wr in requestExports)
            {
                if (groupedExportDetails.TryGetValue(wr.ProductId, out int approvedQty))
                {
                    wr.QuantityApproved = approvedQty;
                    wr.RemainingQuantity = wr.QuantityRequested - approvedQty;
                    wr.Status = "APPROVED";
                    wr.ApprovedBy = approvedBy;
                }
            }

            _context.WarehouseRequestExports.UpdateRange(requestExports);

            // ✅ Cập nhật trạng thái cho RequestExport
            var totalRemaining = requestExports.Sum(x => x.RemainingQuantity);
            var requestExport = await _context.RequestExports
                .FirstOrDefaultAsync(x => x.RequestExportId == receipt.RequestExportId);

            if (requestExport != null)
            {
                requestExport.Status = totalRemaining == 0 ? "Approved" : "Partially_Exported";
                _context.RequestExports.Update(requestExport);
            }

            // ✅ Ghi lại giao dịch
            var exportTransaction = new ExportTransaction
            {
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                ExportDate = receipt.ExportDate,
                ExportType = receipt.ExportType,
                WarehouseId = receipt.WarehouseId,
                Note = "Approved Export",
                RequestExportId = receipt.RequestExportId,
                OrderCode = receipt.RequestExport.Order.OrderCode,
                AgencyName = receipt.RequestExport.Order.RequestProduct.AgencyAccount.AgencyName,
                ExportTransactionDetail = receipt.ExportWarehouseReceiptDetails.Select(d => new ExportTransactionDetail
                {
                    WarehouseProductId = d.WarehouseProductId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    TotalProductAmount = d.TotalProductAmount,
                    ExpiryDate = d.ExpiryDate
                }).ToList()
            };

            _context.ExportTransactions.Add(exportTransaction);

            await _context.SaveChangesAsync();
        }


        public async Task<List<ExportWarehouseReceipt>> GetAllReceiptsByWarehouseIdAsync(long warehouseId)
        {
            return await _context.ExportWarehouseReceipts
                .Where(r => r.WarehouseId == warehouseId)
                .Include(r => r.ExportWarehouseReceiptDetails) // Nếu cần lấy chi tiết sản phẩm
                .ToListAsync();
        }

        public async Task<ExportWarehouseReceipt> CreateFromRequestAsync(int requestExportId, long warehouseId, Guid approvedBy)
        {
            if (await _repository.ReceiptExistsAsync(requestExportId))
                throw new InvalidOperationException($"RequestExportId = {requestExportId} has already been used to create an export receipt.");

            var requestExport = await _repository.GetRequestExportWithDetailsAsync(requestExportId);
            if (requestExport == null)
                throw new KeyNotFoundException("RequestExport not found");

            if (!requestExport.RequestExportDetails.Any())
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
                var availableProducts = await _repository.GetAvailableWarehouseProductsAsync(detail.ProductId, warehouseId);

                if (!availableProducts.Any())
                    throw new InvalidOperationException($"No available stock for ProductId = {detail.ProductId} in WarehouseId = {warehouseId}");

                foreach (var wp in availableProducts)
                {
                    if (quantityNeeded <= 0) break;

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
                    throw new InvalidOperationException($"Not enough stock for ProductId = {detail.ProductId}. Missing: {quantityNeeded}");
            }

            receipt.TotalAmount = totalAmount;

            await _repository.AddExportReceiptAsync(receipt);

            // ✅ Tự động duyệt phiếu vừa tạo
            //ko xoa dc approved nen de do
            await ApproveReceiptAsync(receipt.ExportWarehouseReceiptId, approvedBy);

            return receipt;
        }


        public async Task<bool> UpdateExportReceiptAsync(UpdateExportWarehouseReceiptFullDto dto)
        {
            return await _repository.UpdateFullAsync(dto);
        }
    }

    }
