using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models
{
    [Index(nameof(RequestProductId), nameof(ProductId), IsUnique = true)]
    public class RequestProductDetail
    {
        [Key]
        public long RequestDetailId { get; set; }

        public long RequestProductId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("RequestProductId")]
        public RequestProduct RequestProduct { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
