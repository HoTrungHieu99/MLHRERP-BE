using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class ExportWarehouseReceiptController : ControllerBase
    {
        private readonly IExportWarehouseReceiptService _service;

        public ExportWarehouseReceiptController(IExportWarehouseReceiptService service)
        {
            _service = service;
        }

        // POST: api/ExportWarehouseReceipt
        [HttpPost]
        public async Task<ActionResult<ExportWarehouseReceipt>> CreateExportWarehouseReceiptAsync([FromBody] ExportRequest exportRequest)
        {
            if (exportRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            // Tạo phiếu xuất kho từ request
            var createdReceipt = await _service.CreateExportWarehouseReceiptAsync(exportRequest);

            // Trả về kết quả sau khi lưu thành công
            return CreatedAtAction(nameof(GetExportWarehouseReceiptByIdAsync), new { id = createdReceipt.ExportWarehouseReceiptId }, createdReceipt);
        }

        // GET: api/ExportWarehouseReceipt/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExportWarehouseReceipt>> GetExportWarehouseReceiptByIdAsync(long id)
        {
            var receipt = await _service.GetExportWarehouseReceiptByIdAsync(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return Ok(receipt);
        }
    }*/


}
