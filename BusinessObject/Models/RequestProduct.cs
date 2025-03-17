using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class RequestProduct
    {
        [Key]
        public long RequestProductId { get; set; }
        public long AgencyId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RequestStatus { get; set; } // PENDING, APPROVED, REJECTED

        [ForeignKey("AgencyId")]
        public AgencyAccount AgencyAccount { get; set; }

        [ForeignKey("ApprovedBy")]
        public Employee? ApprovedByEmployee { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
