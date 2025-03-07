using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ImportTransactionDetail
    {
        [Key]
        public long ImportTransactionDetailId { get; set; }
        public int TotalQuantity { get; set; }
        public long ImportTransactionId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }

        [ForeignKey("ImportTransactionId")]
        public ImportTransaction ImportTransaction { get; set; }
        public ICollection<Batch> Batches { get; set; }
    }
}
