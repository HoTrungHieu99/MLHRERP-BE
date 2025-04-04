using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IPaymentRepository
    {
        Task<List<PaymentHistory>> GetPaymentsByOrderIdAsync(Guid orderId);
        Task AddPaymentHistoryAsync(PaymentHistory paymentHistory);
        Task AddPaymentTransactionAsync(PaymentTransaction paymentTransaction);
        Task<string> GetPaymentDetailsAsync(long orderCode);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task SaveChangesAsync();

        Task<PaymentHistory?> GetPaymentHistoryByOrderIdAsync(Guid orderId);
        Task InsertPaymentHistoryAsync(PaymentHistory history);
        Task UpdatePaymentHistoryAsync(PaymentHistory history);
        Task InsertPaymentTransactionAsync(PaymentTransaction transaction);

        Task<PaymentTransaction?> GetTransactionByReferenceAsync(Guid paymentHistoryId);

    }
}
