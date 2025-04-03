using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseTransferRequest
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string RequestCode { get; set; } = string.Empty;

        // 🔹 Kho nguồn: nullable vì được chọn sau
        public long? SourceWarehouseId { get; set; }

        [ForeignKey("SourceWarehouseId")]
        public Warehouse? SourceWarehouse { get; set; }

        // 🔸 Kho đích vẫn bắt buộc
        [Required]
        public long DestinationWarehouseId { get; set; }

        [ForeignKey("DestinationWarehouseId")]
        public Warehouse DestinationWarehouse { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        public Guid RequestedBy { get; set; }

        [ForeignKey("RequestedBy")]
        public User Requester { get; set; }

        public Guid? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public User? Approver { get; set; }

        // 🔹 Người chọn kho nguồn (Planner / điều phối viên)
        public Guid? PlannedBy { get; set; }

        [ForeignKey("PlannedBy")]
        public User? Planner { get; set; }

        [ForeignKey("RequestExport")]
        public int RequestExportId { get; set; }
        public RequestExport RequestExport { get; set; }
        public string? OrderCode { get; set; }


        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Planned, Approved, Completed, etc.

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<WarehouseTransferProduct> TransferProducts { get; set; }

        public ICollection<ExportWarehouseReceipt> ExportWarehouseReceipts { get; set; }

    }
}
