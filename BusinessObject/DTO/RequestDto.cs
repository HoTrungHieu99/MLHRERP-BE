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
        public long AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string RequestStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<RequestItemDto> Items { get; set; }
    }
}
