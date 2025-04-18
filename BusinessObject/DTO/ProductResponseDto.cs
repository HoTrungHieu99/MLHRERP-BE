﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ProductResponseDto
    {
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public int? DefaultExpiration { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public int? TaxId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        // ✅ Danh sách URL hình ảnh lấy từ database
        public List<string> Images { get; set; } = new List<string>();
    }
}
