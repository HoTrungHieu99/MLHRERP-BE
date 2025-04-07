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
                .Include(ph => ph.Order)
                    .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(rp => rp.AgencyAccount)
                        .Include(ph => ph.PaymentTransactions)
                .FirstOrDefaultAsync(ph => ph.PaymentHistoryId == id);
        }


        public async Task<List<PaymentHistory>> GetAllAsync()
        {
            return await _context.PaymentHistories
                .Include(ph => ph.Order)
                    .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(rp => rp.AgencyAccount)
                        .Include(ph => ph.PaymentTransactions)
                .ToListAsync();
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoryByUserIdAsync(Guid userId)
        {
            return await _context.PaymentHistories
                .Include(ph => ph.Order)
                    .ThenInclude(o => o.RequestProduct)
                        .ThenInclude(rp => rp.AgencyAccount)
                        .Include(ph => ph.PaymentTransactions)
                .Where(ph => ph.UserId == userId)
                .ToListAsync();
        }

        public async Task<decimal?> GetCreditLimitByUserIdAsync(Guid userId)
        {
            return await _context.AgencyAccounts
                .Where(aa => aa.UserId == userId)
                .SelectMany(aa => aa.AgencyAccountLevels)
                .OrderByDescending(aal => aal.ChangeDate)
                .Select(aal => aal.Level.CreditLimit)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetPaymentTermByUserIdAsync(Guid userId)
        {
            return await _context.AgencyAccounts
                .Where(aa => aa.UserId == userId)
                .SelectMany(aa => aa.AgencyAccountLevels)
                .OrderByDescending(aal => aal.ChangeDate)
                .Select(aal => aal.Level.PaymentTerm)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetTotalRemainingDebtAmountByUserIdAsync(Guid userId)
        {
            return await _context.PaymentHistories
                .Where(ph => ph.UserId == userId)
                .SumAsync(ph => ph.RemainingDebtAmount);
        }


    }
}
