using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class PaymentHistory
    {
        [Key]
        public Guid PaymentHistoryId { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string SerieNumber { get; set; }
        public string Status { get; set; } // UNPAID, PARTIALLY_PAID, FULL_PAID
        public DateTime DueDate { get; set; }

        [ForeignKey("PrePayment")]
        public Guid? PrePaymentId { get; set; }
        public PaymentHistory PrePayment { get; set; }

        public decimal TotalAmountPayment { get; set; }
        public decimal RemainingDebtAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }


        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
