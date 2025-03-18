﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RequestExportDto
    {
        public int RequestExportId { get; set; }
        public Guid OrderId { get; set; }
        public long RequestedBy { get; set; }
        public long ApprovedBy { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Note { get; set; }
        public List<RequestExportDetailDto> RequestExportDetails { get; set; }
    }
}
