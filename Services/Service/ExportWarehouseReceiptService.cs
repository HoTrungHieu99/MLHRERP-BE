using BusinessObject.DTO;
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
                OrderCode = requestExport.Order.OrderId, // 🔥 Lấy từ RequestExport
                AgencyName = requestExport.Order.RequestProduct.AgencyAccount.AgencyName // 🔥 Lấy từ SalesAgent
            };

            return await _repository.CreateReceiptAsync(receipt);
        }

        public async Task ApproveReceiptAsync(long id)
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
                {
                    throw new Exception($"Insufficient quantity in warehouse for product {warehouseProduct.ProductId}");
                }

                warehouseProduct.Quantity -= detail.Quantity;

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == warehouseProduct.ProductId);
                if (product != null)
                {
                    if (product.AvailableStock < detail.Quantity)
                    {
                        throw new Exception($"Product stock is insufficient for product {product.ProductId}");
                    }

                    product.AvailableStock -= detail.Quantity;
                    _context.Products.Update(product);
                }
            }

            var exportTransaction = new ExportTransaction
            {
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                ExportDate = receipt.ExportDate,
                ExportType = receipt.ExportType,
                WarehouseId = receipt.WarehouseId,
                Note = "Approved Export",
                RequestExportId = receipt.RequestExportId, // 🔥 Thêm RequestExportId
                OrderCode = receipt.RequestExport.Order.OrderId, // 🔥 Lấy OrderCode
                AgencyName = receipt.RequestExport.Order.RequestProduct.AgencyAccount.AgencyName, // 🔥 Lấy AgencyName
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
    }

    }
