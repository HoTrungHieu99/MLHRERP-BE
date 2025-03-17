using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RequestDto
    {
        public long RequestId { get; set; }
        public string RequestStatus { get; set; } // PENDING, APPROVED, REJECTED
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long AgencyId { get; set; }
        public string AgencyName { get; set; }

        // ✅ Danh sách sản phẩm trong request
        public List<RequestProductDto> Products { get; set; } = new List<RequestProductDto>();
    }

    public class RequestProductDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
