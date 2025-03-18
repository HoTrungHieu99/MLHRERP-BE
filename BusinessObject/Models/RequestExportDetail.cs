using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{

    public class RequestExportDetail
    {
        [Key]
        public int RequestItemId { get; set; }

        [ForeignKey("RequestExport")]
        public int RequestExportId { get; set; }
        public RequestExport RequestExport { get; set; }

        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public Product Product { get; set; }

        public int RequestedQuantity { get; set; }
    }
}
