using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.PaymentDTO
{
    public class CreatePaymentRequest
    {
        public Guid? OrderId { get; set; }
        public long AgencyId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

    }
}
