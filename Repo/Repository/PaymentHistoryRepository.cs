using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repo.IRepository;

namespace Repo.Repository
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly MinhLongDbContext _context;

        public PaymentHistoryRepository(MinhLongDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentHistory> GetByIdAsync(Guid id)
        {
            return await _context.PaymentHistories
                .Include(p => p.Order)
                .Include(p => p.PrePayment)
                .Include(p => p.User)
                .Include(p => p.PaymentTransactions)
                .FirstOrDefaultAsync(p => p.PaymentHistoryId == id);
        }

        public async Task<List<PaymentHistory>> GetAllAsync()
        {
            return await _context.PaymentHistories
                .Include(p => p.Order)
                .Include(p => p.PrePayment)
                .Include(p => p.User)
                .Include(p => p.PaymentTransactions)
                .ToListAsync();
        }
    }
}
