using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class WarehouseRequestExport
    {
        [Key]
        public int WarehouseRequestExportId { get; set; }

        [ForeignKey("RequestExport")]
        public int RequestExportId { get; set; }
        public RequestExport RequestExport { get; set; }

        [ForeignKey("Warehouse")]
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int QuantityRequested { get; set; }
        public int? QuantityApproved { get; set; }

        public string Status { get; set; } // PENDING, APPROVED, REJECTED
    }

}
