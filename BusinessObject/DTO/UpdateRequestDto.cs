﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateRequestDto
    {
        public long RequestId { get; set; }
        public int Quantity { get; set; }
        public string RequestStatus { get; set; }
    }
}
