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
    }

}
