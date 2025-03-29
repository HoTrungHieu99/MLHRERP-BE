using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseRequestExportDto
    {
        public int WarehouseRequestExportId { get; set; }
        public int RequestExportId { get; set; }
        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int QuantityRequested { get; set; }
        public int? QuantityApproved { get; set; }
        public int RemainingQuantity { get; set; }
        public string Status { get; set; } // PENDING, APPROVED, REJECTED
        public Guid? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    // 📌 DTO để duyệt số lượng xuất kho
    public class ApproveWarehouseRequestDto
    {
        public int RequestId { get; set; }
        public int QuantityApproved { get; set; }
        public Guid ApprovedBy { get; set; }
    }
}
