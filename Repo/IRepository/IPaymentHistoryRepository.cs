using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IPaymentHistoryRepository
    {
        Task<PaymentHistory> GetByIdAsync(Guid id);
        Task<List<PaymentHistory>> GetAllAsync();
        Task<List<PaymentHistory>> GetPaymentHistoryByUserIdAsync(Guid userId);

        Task<decimal?> GetCreditLimitByUserIdAsync(Guid userId);
        Task<int?> GetPaymentTermByUserIdAsync(Guid userId);

        Task<decimal> GetTotalRemainingDebtAmountByUserIdAsync(Guid userId);

    }

}
