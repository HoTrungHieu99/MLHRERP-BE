﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class WarehouseReceiptRequest
    {
        [Required]
        public string DocumentNumber { get; set; } // Số chứng từ

        [Required]
        public DateTime DocumentDate { get; set; } // Ngày chứng từ

        [Required]
        public long WarehouseId { get; set; } // Kho nhập

        [Required]
        public string ImportType { get; set; } // Loại nhập (Nhập sản xuất, Nhập chuyển, Nhập trả)

        [Required]
        public string Supplier { get; set; } // Nhà cung cấp

        [Required]
        public DateTime DateImport { get; set; } // Ngày nhập

        [Required]
        public string Note { get; set; } = "nothing";

        // Không nhập từ người dùng, sẽ tính toán tự động
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Danh sách chi tiết nhập hàng
        [Required]
        public List<BatchRequest> Batches { get; set; } = new List<BatchRequest>();
    }

}
