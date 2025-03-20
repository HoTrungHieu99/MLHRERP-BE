using BusinessObject.DTO;
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
            await _service.ApproveReceiptAsync(id);
            return Ok("Receipt Approved");
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
    }
}
