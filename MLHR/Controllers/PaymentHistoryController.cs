using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IPaymentHistoryService _service;

        public PaymentHistoryController(IPaymentHistoryService service)
        {
            _service = service;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllPaymentHistoriesAsync();
            return Ok(data);
        }

        [HttpGet("Payment-History-id/{PaymentHistoryId}")]
        public async Task<IActionResult> GetById(Guid PaymentHistoryId)
        {
            var data = await _service.GetPaymentHistoryByIdAsync(PaymentHistoryId);
            if (data == null) return NotFound("PaymentHistory not found.");
            return Ok(data);
        }

        [HttpGet("my-payment-history")]
        public async Task<IActionResult> GetPaymentHistoryForLoggedInUser()
        {
            var userId = GetLoggedInUserId(); // 🟢 Hàm này nên lấy từ JWT claims
            if (userId == null)
            {
                return Unauthorized(new { message = "User chưa đăng nhập." });
            }

            var histories = await _service.GetPaymentHistoriesByUserIdAsync(userId.Value);
            if (histories == null || !histories.Any())
            {
                return NotFound(new { message = "Không có lịch sử thanh toán nào." });
            }

            return Ok(histories);
        }

        private Guid? GetLoggedInUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            return Guid.TryParse(userIdClaim.Value, out var id) ? id : null;
        }


    }

}
