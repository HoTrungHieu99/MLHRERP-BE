using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseReceiptController : ControllerBase
    {
        private readonly IWarehouseReceiptService _service;

        public WarehouseReceiptController(IWarehouseReceiptService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _service.GetAllReceiptsAsync();
            return Ok(receipts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] WarehouseReceiptRequest request)
        {
            var result = await _service.CreateReceiptAsync(request);
            if (!result)
            {
                return BadRequest("Save entry failed!");
            }
            return Ok("Save entry form successfully!");
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(long id)
        {
            var result = await _service.ApproveReceiptAsync(id);
            return result ? Ok("Approved!") : NotFound("Receipt not found");
        }
    }

}
