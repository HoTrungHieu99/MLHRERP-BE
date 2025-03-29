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
    }

}
