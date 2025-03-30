using System.Security.Claims;
using BusinessObject.DTO.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Exceptions;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "3")]
    public class WarehouseReceiptController : ControllerBase
    {
        private readonly IWarehouseReceiptService _service;

        public WarehouseReceiptController(IWarehouseReceiptService service)
        {
            _service = service;
        }

        [HttpGet("by-warehouse/{warehouseId}")]
        public async Task<IActionResult> GetAllByWarehouseId(long warehouseId, [FromQuery] string? sortBy)
        {
            var receipts = await _service.GetAllReceiptsByWarehouseIdAsync(warehouseId, sortBy);
            return Ok(receipts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] WarehouseReceiptRequest request)
        {
            try
            {
                // ✅ Lấy userId từ token
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

                // ✅ Gọi Service để tạo phiếu nhập kho
                var result = await _service.CreateReceiptAsync(request, userId);

                if (!result)
                {
                    return BadRequest(new { success = false, message = "Lưu phiếu nhập thất bại!" });
                }

                return Ok(new { success = true, message = "Lưu phiếu nhập thành công!" });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống! Vui lòng thử lại sau." });
            }
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(long id)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var result = await _service.ApproveReceiptAsync(id, currentUserId);
                return result ? Ok(new { success = true, message = "Approved!" }) : NotFound(new { success = false, message = "Receipt not found" });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống! Vui lòng thử lại sau." });
            }


        }


        [HttpGet("{warehouseReceiptId}")]
        public async Task<IActionResult> GetWarehouseReceiptById(long warehouseReceiptId)
        {
            try
            {
                var requestProduct = await _service.GetWarehouseReceiptDTOIdAsync(warehouseReceiptId);
                return Ok(requestProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
