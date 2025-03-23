using System.Security.Claims;
using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouseId(long warehouseId)
        {
            var results = await _service.GetByWarehouseIdAsync(warehouseId);
            if (results == null || !results.Any())
            {
                return NotFound("No warehouse request exports found for the given warehouse ID.");
            }

            return Ok(results);
        }


        [HttpPost("approve")]
        public async Task<IActionResult> ApproveRequest([FromBody] ApproveWarehouseRequestExportDto requestDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Invalid user ID" });

                var result = await _service.ApproveRequestAsync(
                    requestDto.WarehouseRequestExportId,
                    requestDto.QuantityApproved,
                    userId
                );

                if (result)
                    return Ok(new { message = "Request approved successfully" });

                return BadRequest(new { message = "Approval failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
