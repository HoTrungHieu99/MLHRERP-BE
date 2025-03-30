using BusinessObject.DTO.Warehouse;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/export-receipts")]
    [ApiController]
    [Authorize] // Chỉ người dùng đăng nhập mới có quyền truy cập
    public class ExportWarehouseReceiptController : ControllerBase
    {
        private readonly IExportWarehouseReceiptService _service;
        public ExportWarehouseReceiptController(IExportWarehouseReceiptService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> CreateReceipt([FromBody] ExportWarehouseReceiptDTO dto)
        {
            var receipt = await _service.CreateReceiptAsync(dto);
            return Ok(receipt);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> ApproveReceipt(long id)
        {
            try
            {
                // ✅ Lấy userId từ JWT token (ClaimTypes.NameIdentifier)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized("Invalid or missing user ID.");
                }

                // ✅ Gọi service và truyền userId vào
                await _service.ApproveReceiptAsync(id, userId);

                return Ok("Receipt Approved");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-all/{warehouseId}")]
        public async Task<IActionResult> GetAllByWarehouseId(long warehouseId)
        {
            var receipts = await _service.GetAllReceiptsByWarehouseIdAsync(warehouseId);
            if (receipts == null || !receipts.Any())
            {
                return NotFound(new { message = "No export receipts found for this warehouse." });
            }
            return Ok(receipts);
        }

        [HttpPost("create-from-request")]
        public async Task<IActionResult> CreateFromRequest([FromQuery] int requestExportId, [FromQuery] long warehouseId)
        {
            try
            {
                var receipt = await _service.CreateFromRequestAsync(requestExportId, warehouseId);
                return Ok(receipt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update-full")]
        public async Task<IActionResult> UpdateFull([FromBody] UpdateExportWarehouseReceiptFullDto dto)
        {
            try
            {
                var success = await _service.UpdateExportReceiptAsync(dto);
                return success ? Ok("Update successful") : BadRequest("Update failed");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
