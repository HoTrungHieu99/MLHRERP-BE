using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repo.IRepository
{
    public interface IPaymentTransactionRepository
    {
        Task<IEnumerable<PaymentTransaction>> GetAllAsync();
        Task<PaymentTransaction> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(Guid userId);
    }
}
