using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Product
{
    public class RequestProductDto
    {
        public Guid RequestProductId { get; set; }
        public string RequestCode { get; set; }
        public long AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string ApprovedName { get; set; }
        public long? ApprovedBy { get; set; }
        public string RequestStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RequestProductDetailDto> RequestProductDetails { get; set; }
    }

}
