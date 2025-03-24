using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTO.PaymentDTO
{
    public class CreatePaymentRequest
    {
        public Guid? OrderId { get; set; }
        public long AgencyId { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public string? Description { get; set; }

    }
}
