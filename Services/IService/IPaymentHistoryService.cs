using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.PaymentDTO;
using BusinessObject.Models;

namespace Services.IService
{
    public interface IPaymentHistoryService
    {
        Task<PaymentHistoryDto> GetPaymentHistoryByIdAsync(Guid id);
        Task<List<PaymentHistoryDto>> GetAllPaymentHistoriesAsync();
    }
}
