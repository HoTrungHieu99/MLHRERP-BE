﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CreateRequestDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
