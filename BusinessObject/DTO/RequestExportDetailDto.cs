using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RequestExportDetailDto
    {
        public int RequestExportDetailId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int RequestedQuantity { get; set; }
    }

}
