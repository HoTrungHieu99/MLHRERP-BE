using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repo.IRepository;
using Repo.Repository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class WarehouseReceiptService : IWarehouseReceiptService
    {
        private readonly IWarehouseReceiptRepository _repository;

        public WarehouseReceiptService(IWarehouseReceiptRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateReceiptAsync(WarehouseReceiptRequest request)
        {
            var validateImportType = new HashSet<string>
            {
                "Nhập Sản Xuất", 
                "Nhập trả hàng", 
                "Nhập Mua", 
                "Nhập bổ sung"
            };

            if (!validateImportType.Contains(request.ImportType))
            {
                throw new Exception("ImportType is invalid! Only accepted: Nhập Sản Xuất, Nhập trả hàng, Nhập Mua, Nhập bổ sung!");
            }

            // ✅ Tính toán TotalAmount cho từng batch
            var processedBatches = request.Batches.Select(b => new BatchResponseDto
            {
                BatchCode = b.BatchCode,
                ProductId = b.ProductId,
                Unit = b.Unit,
                Quantity = b.Quantity,
                UnitCost = b.UnitCost,
                TotalAmount = b.Quantity * b.UnitCost, // ✅ Tính toán tổng giá trị
                Status = b.Status
            }).ToList();

            // ✅ Tính TotalPrice bằng tổng TotalAmount của tất cả batch
            int totalQuantity = processedBatches.Sum(b => b.Quantity);
            decimal totalPrice = processedBatches.Sum(b => b.TotalAmount); // ✅ Tổng tất cả TotalAmount
            DateTime documentDate = DateTime.Now;
            DateTime ImportDate = DateTime.Now;

            // ✅ Chuyển danh sách thành JSON
            string batchesJson = JsonConvert.SerializeObject(processedBatches, Formatting.Indented);

            var warehouseReceipt = new WarehouseReceipt
            {
                DocumentNumber = request.DocumentNumber,
                DocumentDate = documentDate,
                WarehouseId = request.WarehouseId,
                ImportType = request.ImportType,
                Supplier = request.Supplier,
                DateImport = ImportDate,
                TotalQuantity = totalQuantity,  // ✅ Đã được tính toán
                TotalPrice = totalPrice,        // ✅ Đã được tính toán
                BatchesJson = batchesJson       // ✅ Gán chuỗi JSON đúng cách
            };

            return await _repository.AddAsync(warehouseReceipt);
        }


        public async Task<bool> ApproveReceiptAsync(long id)
        {
            return await _repository.ApproveAsync(id);
        }

        public async Task<List<WarehouseReceiptDTO>> GetAllReceiptsAsync()
        {
            var receipts = await _repository.GetAllAsync();

            return receipts.Select(receipt => new WarehouseReceiptDTO
            {
                DocumentNumber = receipt.DocumentNumber,
                DocumentDate = receipt.DocumentDate,
                WarehouseId = receipt.WarehouseId,
                ImportType = receipt.ImportType,
                Supplier = receipt.Supplier,
                DateImport = receipt.DateImport,
                TotalQuantity = receipt.TotalQuantity,
                TotalPrice = receipt.TotalPrice,
                Batches = JsonConvert.DeserializeObject<List<BatchResponseDto>>(receipt.BatchesJson) ?? new List<BatchResponseDto>() // ✅ Chuyển JSON thành danh sách BatchDTO
            }).ToList();
        }
    }

}
