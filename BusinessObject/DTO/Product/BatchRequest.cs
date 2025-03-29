﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Product
{
    public class BatchRequest
    {
        public string? BatchCode { get; set; } // Mã lô

        [Required]
        public long ProductId { get; set; } // Mã sản phẩm

        [Required]
        public string Unit { get; set; } // Đơn vị tính

        [Required]
        public int Quantity { get; set; } // Số lượng

        [Required]
        public decimal UnitCost { get; set; } // Đơn giá

        [JsonIgnore]
        public decimal TotalAmount { get; set; } // Thành tiền

        public string? Status { get; set; } // Trạng thái (Có sẵn, Hết hạn)
        [Required]
        public DateTime DateOfManufacture { get; set; } // ✅ Ngày sản xuất (Bắt buộc nhập)

    }

}
