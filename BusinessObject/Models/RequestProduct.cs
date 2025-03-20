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
        public Guid RequestProductId { get; set; } // Khóa chính
        public long RequestCode { get; set; }
        public long AgencyId { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string RequestStatus { get; set; } // PENDING, APPROVED, REJECTED

        [ForeignKey("AgencyId")]
        public AgencyAccount AgencyAccount { get; set; }

        [ForeignKey("ApprovedBy")]
        public Employee? ApprovedByEmployee { get; set; }

        // Mối quan hệ 1-1 với Order
        public Order Order { get; set; }

        public ICollection<RequestProductDetail> RequestProductDetails { get; set; }
    }
}
