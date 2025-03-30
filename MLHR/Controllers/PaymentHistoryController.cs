using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

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

        [HttpGet("{Payment-History-id}")]
        public async Task<IActionResult> GetById(Guid PaymentHistoryId)
        {
            var data = await _service.GetPaymentHistoryByIdAsync(PaymentHistoryId);
            if (data == null) return NotFound("PaymentHistory not found.");
            return Ok(data);
        }

        
    }

}
