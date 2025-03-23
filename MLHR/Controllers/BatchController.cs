using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.IService;
using SkiaSharp;

namespace MLHR.Controllers
{
    [Route("api/batch")]
    [ApiController]
    [Authorize(Roles = "3, 4")]
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
            var result = await _batchService.UpdateProfitMarginAsync(batchId, profitMarginPercent);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
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
