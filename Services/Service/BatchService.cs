using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.Product;
using BusinessObject.Models;
using Repo.IRepository;
using Services.IService;

namespace Services.Service
{
    public class BatchService : IBatchService
    {
        private readonly IBatchRepository _batchRepository;

        public BatchService(IBatchRepository batchRepository)
        {
            _batchRepository = batchRepository;
        }

        public async Task<Batch> GetBatchByIdAsync(long batchId)
        {
            return await _batchRepository.GetByIdAsync(batchId);
        }

        public async Task<IEnumerable<Batch>> GetAllBatchesAsync()
        {
            return await _batchRepository.GetAllAsync();
        }

        public async Task<bool> UpdateBatchAsync(Batch batch)
        {
            return await _batchRepository.UpdateAsync(batch);
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductIdAsync(long productId)
        {
            return await _batchRepository.GetBatchesByProductIdAsync(productId);
        }

        public async Task<(bool Success, string Message, object? Data)> UpdateProfitMarginAsync(long batchId, decimal profitMarginPercent)
        {
            if (profitMarginPercent < 0)
                return (false, "Profit margin percentage must be greater than or equal to 0.", null);

            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null)
                return (false, "Batch not found.", null);

            if (batch.Status != "CALCULATING_PRICE")
                return (false, "Cannot update profit margin. Batch is not in 'CALCULATING_PRICE' state.", null);

            batch.ProfitMarginPercent = profitMarginPercent;
            batch.SellingPrice = batch.UnitCost * (1 + (profitMarginPercent / 100));
            batch.Status = "ACTIVE";

            // ✅ Gọi repo xử lý update + đồng bộ
            bool updated = await _batchRepository.UpdateBatchAndRelatedDataAsync(batch);

            if (!updated)
                return (false, "Failed to update batch and related data.", null);

            return (true, "Success", new
            {
                batch.BatchId,
                batch.BatchCode,
                batch.UnitCost,
                ProfitMarginPercent = batch.ProfitMarginPercent,
                SellingPrice = batch.SellingPrice,
                Status = batch.Status
            });
        }

        public async Task<ProductInfoByBatchDto?> GetProductInfoByBatchIdAsync(long batchId)
        {
            var batch = await _batchRepository.GetByIdAsync(batchId);

            if (batch == null) return null;

            return new ProductInfoByBatchDto
            {
                ProductName = batch.Product?.ProductName ??
                              (await _batchRepository.GetProductByIdAsync(batch.ProductId))?.ProductName,
                UnitCost = batch.UnitCost
            };
        }
    }
}
