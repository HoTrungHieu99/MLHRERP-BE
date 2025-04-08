using BusinessObject.DTO.RequestExport;
using System.Security.Claims;
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
        public async Task<IActionResult> GetAllByWarehouseId(long warehouseId, [FromQuery] string? sortBy)
        {
            var receipts = await _service.GetAllReceiptsByWarehouseIdAsync(warehouseId, sortBy);
            if (receipts == null || !receipts.Any())
            {
                return NotFound(new { message = "No export receipts found for this warehouse." });
            }
            return Ok(receipts);
        }

        [Authorize] // ✅ Đảm bảo chỉ user đăng nhập mới được gọi API này
        [HttpPost("create-from-request")]
        public async Task<IActionResult> CreateFromRequest([FromQuery] int requestExportId, [FromQuery] long warehouseId)
        {
            try
            {
                // ✅ Lấy userId từ JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized("Invalid or missing user ID.");
                }

                // ✅ Gọi service (đã tích hợp auto approve)
                var receipt = await _service.CreateFromRequestAsync(requestExportId, warehouseId, userId);

                return Ok(receipt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
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

        [HttpPost("from-transfer")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> CreateFromTransfer([FromBody] ExportWarehouseTransferDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized("Invalid or missing user ID");

                var receipt = await _service.CreateReceiptFromTransferAsync(dto, userId);

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", detail = ex.Message });
            }
        }

        [HttpGet("export-pdf/{id}")]
        public async Task<IActionResult> ExportPdf(long id)
        {
            try
            {
                var pdfBytes = await _service.ExportReceiptToPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"PhieuXuatKho_{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
