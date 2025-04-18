﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ProductDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public int? DefaultExpiration { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public int? TaxId { get; set; }

        // ✅ Danh sách hình ảnh
        public List<string> Images { get; set; } = new List<string>();
    }
}
