using BusinessObject.DTO.PaymentDTO;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IPaymentService
    {
        Task<StatusPayment> ConfirmPayment(string queryString, QueryRequest requestquery);

        Task<CreatePaymentResult> SendPaymentLink(Guid accountId, CreatePaymentRequest request);
    }
}
