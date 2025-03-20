using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseRequestExportController : ControllerBase
    {
        private readonly IWarehouseRequestExportService _service;

        public WarehouseRequestExportController(IWarehouseRequestExportService service)
        {
            _service = service;
        }

        // ✅ API chỉ nhận tham số từ query string
        [HttpPost("create")]
        public async Task<IActionResult> CreateWarehouseRequestExport([FromQuery] long warehouseId, [FromQuery] int requestExportId)
        {
            var result = await _service.CreateWarehouseRequestExportAsync(warehouseId, requestExportId);

            if (result == null)
                return BadRequest("Không thể tạo WarehouseRequestExport. Kiểm tra lại RequestExportId.");

            return Ok(result);
        }
    }
}
