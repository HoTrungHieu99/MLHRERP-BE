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
            // ✅ Tính toán TotalQuantity và TotalPrice ngay từ đầu
            int totalQuantity = request.Batches.Sum(b => b.Quantity);
            decimal totalPrice = request.Batches.Sum(b => b.TotalAmount); // ✅ Tính toán từ Quantity * UnitCost

            // ✅ Tạo danh sách batches mới (bỏ TotalAmount)
            var filteredBatches = request.Batches.Select(b => new
            {
                b.BatchCode,
                b.ProductId,
                b.Unit,
                b.Quantity,
                b.UnitCost,
                b.Status
            }).ToList();

            // ✅ Chuyển danh sách thành JSON
            string batchesJson = JsonConvert.SerializeObject(filteredBatches);

            var warehouseReceipt = new WarehouseReceipt
            {
                DocumentNumber = request.DocumentNumber,
                DocumentDate = request.DocumentDate,
                WarehouseId = request.WarehouseId,
                ImportType = request.ImportType,
                Supplier = request.Supplier,
                DateImport = request.DateImport,
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

        public async Task<List<WarehouseReceipt>> GetAllReceiptsAsync()
        {
            return await _repository.GetAllAsync();
        }
    }

}
