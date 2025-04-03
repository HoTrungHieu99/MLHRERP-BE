using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseTransferRequestCreateDto
    {
        public long? SourceWarehouseId { get; set; }
        public long DestinationWarehouseId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Notes { get; set; }
        public int RequestExportId { get; set; }

        public List<WarehouseTransferProductDto> Products { get; set; }
    }
}
