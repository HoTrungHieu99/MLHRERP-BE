using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseInfoDto
    {
        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string FullAddress { get; set; }
    }
}
