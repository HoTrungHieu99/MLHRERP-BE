using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IService;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api/payment-PayOs")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }



        [HttpPost("{orderId}/payment")]
        public async Task<IActionResult> ProcessPayment(Guid orderId, [FromBody] decimal paymentAmount)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(orderId, paymentAmount);
                if (result)
                {
                    return Ok(new { message = "Thanh toán đã được xử lý thành công!" });
                }

                return BadRequest(new { message = "Thanh toán không thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}/qrcode")]
        public async Task<IActionResult> GetPaymentQRCode(Guid orderId)
        {
            try
            {
                var qrCodeUrl = await _paymentService.GetPaymentQRCodeAsync(orderId);
                return Ok(new { message = "Lấy QR Code thành công", qrCode = qrCodeUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
