using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseRequestExportCreateDto
    {
        public Guid UserId { get; set; } // Người gửi yêu cầu (employee → userId)
        public int RequestExportId { get; set; }
        public long WarehouseId { get; set; }
    }
}
