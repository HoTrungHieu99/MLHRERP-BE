using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.PaymentDTO
{
    public class PaymentTransactionDto
    {
        public Guid TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionReference { get; set; }
    }
}
