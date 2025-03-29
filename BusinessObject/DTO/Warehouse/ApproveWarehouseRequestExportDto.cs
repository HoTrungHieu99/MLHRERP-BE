using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class ApproveWarehouseRequestExportDto
    {
        public int WarehouseRequestExportId { get; set; }
        public int QuantityApproved { get; set; }
        public int RequestExportId { get; set; }    // Thêm dòng này
        public long ProductId { get; set; }
        public Guid ApprovedBy { get; set; } // ID của người duyệt
    }
}
