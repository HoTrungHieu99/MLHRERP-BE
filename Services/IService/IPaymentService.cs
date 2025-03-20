using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(Guid orderId, decimal paymentAmount);

        Task<string> GetPaymentQRCodeAsync(Guid orderId);
    }
}
