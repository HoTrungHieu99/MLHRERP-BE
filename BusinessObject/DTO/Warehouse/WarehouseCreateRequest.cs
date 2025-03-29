using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Warehouse
{
    public class WarehouseCreateRequest
    {
        [Required]
        public string WarehouseName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Ward { get; set; }  // Tên phường/xã
        [Required]
        public string District { get; set; }  // Tên quận/huyện
        [Required]
        public string Province { get; set; }  // Tên tỉnh/thành phố

        public string Note { get; set; }  // Ghi chú thêm cho kho
    }
}
