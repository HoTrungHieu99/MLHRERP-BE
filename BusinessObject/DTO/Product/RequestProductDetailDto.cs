using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Product
{
    public class RequestProductDetailDto
    {
        public long ProductId { get; set; } // ID sản phẩm
        public string ProductName { get;set; }
        public string Unit { get; set; } // Đơn vị tính
        public int Quantity { get; set; } // Số lượng yêu cầu
        public decimal UnitPrice { get; set; } // Đơn giá (nếu có)
    }
}
