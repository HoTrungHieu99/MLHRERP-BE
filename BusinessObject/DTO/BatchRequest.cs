using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class BatchRequest
    {
        [Required]
        public string BatchCode { get; set; } // Mã lô

        [Required]
        public long ProductId { get; set; } // Mã sản phẩm

        [Required]
        public string Unit { get; set; } // Đơn vị tính

        [Required]
        public int Quantity { get; set; } // Số lượng

        [Required]
        public decimal UnitCost { get; set; } // Đơn giá

        [Required]
        public decimal TotalAmount { get; set; } // Thành tiền

        [Required]
        public string Status { get; set; } // Trạng thái (Có sẵn, Hết hạn)

        
    }

}
