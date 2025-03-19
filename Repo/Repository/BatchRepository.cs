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
    }

}
