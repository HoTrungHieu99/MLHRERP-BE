using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CreateRequestProductDto
    {
        //public string RequestNumber { get; set; } // Mã số Request
        public string AgencyName { get; set; }
        public string Note { get; set; } // Ghi chú của Request
        public List<RequestProductDetailDto> Products { get; set; }
    }

}
