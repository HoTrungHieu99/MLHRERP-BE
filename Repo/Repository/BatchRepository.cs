﻿using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repo.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repository
{
    public class BatchRepository : IBatchRepository
    {
        private readonly MinhLongDbContext _context;

        public BatchRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<Batch?> GetLatestBatchByProductIdAsync(long productId)
        {
            return await _context.Batches
                .Where(b => b.ProductId == productId)
                .OrderByDescending(b => b.ExpiryDate)
                .AsQueryable() // ✅ Chuyển về IQueryable trước khi gọi FirstOrDefaultAsync
                .FirstOrDefaultAsync();
        }

        public async Task<Batch> GetByIdAsync(long batchId)
        {
            return await _context.Batches.FindAsync(batchId);
        }

        public async Task<IEnumerable<Batch>> GetAllAsync()
        {
            return await _context.Batches.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Batch batch)
        {
            _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductIdAsync(long productId)
        {
            return await _context.Batches
                .Where(b => b.ProductId == productId)
                .ToListAsync();
        }

        public async Task<int> CountBatchesByDateAsync(DateTime date)
        {
            string datePart = date.ToString("yyyyMMdd");

            return await _context.Batches
                .CountAsync(b => b.BatchCode.StartsWith($"BA{datePart}"));
        }

        public async Task<bool> UpdateBatchAndRelatedDataAsync(Batch batch)
        {
            // ✅ Cập nhật chính bản ghi Batch
            _context.Batches.Update(batch);

            // ✅ Cập nhật trạng thái các dòng WarehouseProduct liên quan
            var warehouseProducts = await _context.WarehouseProduct
                .Where(wp => wp.BatchId == batch.BatchId)
                .ToListAsync();

            foreach (var wp in warehouseProducts)
            {
                wp.Status = batch.Status;
            }

            // ✅ Cập nhật trạng thái trong BatchesJson của WarehouseReceipt
            var warehouseReceipt = await _context.WarehouseReceipts
                .Where(wr => wr.BatchesJson.Contains(batch.BatchCode))
                .FirstOrDefaultAsync();

            if (warehouseReceipt != null && !string.IsNullOrEmpty(warehouseReceipt.BatchesJson))
            {
                var batchList = JsonConvert.DeserializeObject<List<BatchRequest>>(warehouseReceipt.BatchesJson);

                // ✅ Cập nhật tất cả các batch có cùng BatchCode
                foreach (var b in batchList.Where(b => b.BatchCode == batch.BatchCode))
                {
                    b.Status = batch.Status;
                }

                warehouseReceipt.BatchesJson = JsonConvert.SerializeObject(batchList, Formatting.Indented);
            }

            var product = await _context.Products.FindAsync(batch.ProductId);
            if (product != null)
            {
                product.Price = batch.SellingPrice;
                _context.Products.Update(product);
            }

            // ✅ Lưu tất cả thay đổi
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
