using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ImportTransaction
    {
        [Key]
        public long ImportTransactionId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public string TypeImport { get; set; } // Standard, Return, Transfer
        public string? Note { get; set; }
        public long WarehouseId { get; set; }
        public string Supplier { get; set; }
        public DateTime DateImport { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }
        public ICollection<ImportTransactionDetail> ImportTransactionDetails { get; set; }
    }
}
