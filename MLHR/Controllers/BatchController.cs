using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/batch")]
    [ApiController]
    [Authorize(Roles = "4")]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpPut("update-profit-margin/{batchId}/{profitMarginPercent}")]
        public async Task<IActionResult> UpdateProfitMargin(long batchId, decimal profitMarginPercent)
        {
            if (profitMarginPercent < 0)
                return BadRequest("Profit margin percentage must be greater than or equal to 0.");

            var batch = await _batchService.GetBatchByIdAsync(batchId);
            if (batch == null)
                return NotFound("Batch not found.");

            if (batch.Status != "CALCULATING_PRICE")
                return BadRequest("Cannot update profit margin. Batch is not in 'CALCULATING_PRICE' state.");

            // ✅ Tính toán giá bán mới dựa trên lợi nhuận nhập vào
            batch.ProfitMarginPercent = profitMarginPercent;
            batch.SellingPrice = batch.UnitCost * (1 + (profitMarginPercent / 100));

            // ✅ Cập nhật trạng thái thành 'ACTIVE'
            batch.Status = "ACTIVE";

            // ✅ Cập nhật thông tin Batch thông qua Service
            await _batchService.UpdateBatchAsync(batch);

            return Ok(new
            {
                batch.BatchId,
                batch.BatchCode,
                batch.UnitCost,
                ProfitMarginPercent = batch.ProfitMarginPercent,
                SellingPrice = batch.SellingPrice,
                Status = batch.Status
            });
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetBatchesByProduct(long productId)
        {
            var batches = await _batchService.GetBatchesByProductIdAsync(productId);

            if (batches == null || !batches.Any())
                return NotFound("No batches found for the given product.");

            return Ok(batches);
        }
    }
}
