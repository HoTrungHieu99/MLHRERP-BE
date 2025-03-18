using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/export-receipts")]
    [ApiController]
    public class ExportWarehouseReceiptController : ControllerBase
    {
        private readonly IExportWarehouseReceiptService _service;
        public ExportWarehouseReceiptController(IExportWarehouseReceiptService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt([FromBody] ExportWarehouseReceiptDTO dto)
        {
            var receipt = await _service.CreateReceiptAsync(dto);
            return Ok(receipt);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveReceipt(long id)
        {
            await _service.ApproveReceiptAsync(id);
            return Ok("Receipt Approved");
        }
    }
}
