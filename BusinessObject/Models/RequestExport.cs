using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class RequestExport
    {
        [Key]
        public int RequestExportId { get; set; }

        [ForeignKey("RequestedByEmployee")]
        public long RequestedBy { get; set; }
        public Employee RequestedByEmployee { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected

        [ForeignKey("ApprovedByEmployee")]
        public long? ApprovedBy { get; set; }
        public Employee ApprovedByEmployee { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public string Note { get; set; }

        [ForeignKey("Order")]
        public Guid? OrderId { get; set; }
        public Order Order { get; set; }

        public ICollection<RequestExportDetail> RequestExportDetails { get; set; }
        public ICollection<WarehouseRequestExport> WarehouseRequestExports { get; set; }
    }

}
