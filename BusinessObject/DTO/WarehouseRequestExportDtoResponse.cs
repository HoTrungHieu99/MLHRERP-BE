﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class WarehouseRequestExportDtoResponse
    {
        public int WarehouseRequestExportId { get; set; }
        public int RequestExportId { get; set; }
        public long ProductId { get; set; }
        public int QuantityRequested { get; set; }
        public int RemainingQuantity { get; set; }
        public string ProductName { get; set; }

        public string OrderCode { get; set; }
        public string AgencyName { get; set; }

        public string Status { get; set; }
    }
}
