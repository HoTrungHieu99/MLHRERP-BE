using System.Security.Claims;
using BusinessObject.DTO.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseTransferController : ControllerBase
    {
        private readonly IWarehouseTransferService _service;

        public WarehouseTransferController(IWarehouseTransferService service)
        {
            _service = service;
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehouseTransferRequestCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _service.CreateTransferRequestAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "3, 6")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Roles = "3, 6")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "6")]
        [HttpPut("plan/{requestId}/{warehouseId}")]
        public async Task<IActionResult> PlanTransferRequest(long requestId, long warehouseId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            var success = await _service.PlanTransferRequestAsync(requestId, warehouseId, userId);
            return success ? Ok("Kho nguồn đã được chọn thành công.") : NotFound("Yêu cầu không tồn tại.");
        }

      /*  [Authorize(Roles = "3, 6")]
        [HttpGet("to-export/{sourceWarehouseId}")]
        public async Task<IActionResult> GetRequestsToExport(long sourceWarehouseId)
        {
            var result = await _service.GetRequestsToExportAsync(sourceWarehouseId);
            return Ok(result);
        }*/

        [Authorize(Roles = "3")]
        [HttpPost("auto-create-from-remaining")]
        public async Task<IActionResult> AutoCreateFromRemaining([FromBody] AutoCreateTransferRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var requestedBy = Guid.Parse(userIdClaim);

            try
            {
                var result = await _service.AutoCreateTransferRequestFromRemainingAsync(dto, requestedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "3")]
        [HttpGet("by-source/{sourceWarehouseId}")]
        public async Task<IActionResult> GetBySourceWarehouse(long sourceWarehouseId)
        {
            var result = await _service.GetBySourceWarehouseAsync(sourceWarehouseId);
            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpGet("by-destination/{destinationWarehouseId}")]
        public async Task<IActionResult> GetByDestinationWarehouse(long destinationWarehouseId)
        {
            var result = await _service.GetByDestinationWarehouseAsync(destinationWarehouseId);
            return Ok(result);
        }

    }

}
