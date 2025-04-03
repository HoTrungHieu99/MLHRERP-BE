using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseTransferProductDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}
