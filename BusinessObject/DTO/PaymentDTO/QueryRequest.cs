using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.PaymentDTO
{
    public class QueryRequest
    {
        public string? Paymentlink { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? des { get; set; }

        public string? userId { get; set; }


        public decimal price { get; set; }

        public int orderCode { get; set; }
        public string? Url { get; set; }

        // 👇 Thêm thuộc tính này
        public Guid OrderId { get; set; }
    }
}
