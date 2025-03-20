using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PayOSPaymentResponse
    {
        [JsonProperty("orderCode")]
        public long OrderCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }


}
