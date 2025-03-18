using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class PaymentTransaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [ForeignKey("PaymentHistory")]
        public Guid PaymentHistoryId { get; set; }
        public PaymentHistory PaymentHistory { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // SUCCESS, FAIL
        public string TransactionReference { get; set; }
    }
}
