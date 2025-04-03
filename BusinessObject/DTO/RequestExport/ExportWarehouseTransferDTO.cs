using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.RequestExport
{
    public class ExportWarehouseTransferDTO
    {
        public long WarehouseTransferRequestId { get; set; } // yêu cầu điều phối
        public long SourceWarehouseId { get; set; } // kho xuất
        public long DestinationWarehouseId { get; set; } // kho nhận
        public DateTime ExportDate { get; set; }
    }
}
