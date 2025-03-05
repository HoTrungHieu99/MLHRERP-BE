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
            string batchesJson = JsonConvert.SerializeObject(request.Batches); // ✅ Lưu batch dưới dạng JSON

            var warehouseReceipt = new WarehouseReceipt
            {
                DocumentNumber = request.DocumentNumber,
                DocumentDate = request.DocumentDate,
                WarehouseId = request.WarehouseId,
                ImportType = request.ImportType,
                Supplier = request.Supplier,
                DateImport = request.DateImport,
                TotalQuantity = request.TotalQuantity,
                TotalPrice = request.TotalPrice,
                BatchesJson = batchesJson
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
