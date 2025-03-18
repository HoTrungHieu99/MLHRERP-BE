using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ProductSimpleResponseDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public List<string> Images { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
