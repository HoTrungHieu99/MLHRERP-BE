using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.PaymentDTO
{
    public class PayOSPaymentRequest
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public List<PayOSItemData> Items { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}
