using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IPaymentTransactionService
    {
        Task<IEnumerable<PaymentTransactionDto>> GetAllAsync();
        Task<PaymentTransactionDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentTransactionDto>> GetByUserIdAsync(Guid userId);

    }

}
