using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Product
{
    public class CreateRequestProductDetailDto
    {
        public long ProductId { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
    }
}
