using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Accountant
    {
        [Key]
        public long AccountantId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        // Quan hệ 1-1 với User
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Quan hệ n-1 với Address
        public int? AddressId { get; set; }
        public Address Address { get; set; }

        // Quan hệ 1-n với PaymentHistory (nếu cần)
        public ICollection<PaymentHistory> PaymentHistories { get; set; }
    }

}
