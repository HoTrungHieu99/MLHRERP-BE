using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        // 🔹 Lấy UserId từ JWT Token
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        // 🔹 Lấy danh sách Request
        [HttpGet]
        [Authorize(Roles = "4")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAllRequests()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        // 🔹 Lấy chi tiết một Request
        [HttpGet("{id}")]
        [Authorize(Roles = "4")]
        public async Task<ActionResult<RequestDto>> GetRequestById(long id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null) return NotFound(new { message = "Request not found." });
            return Ok(request);
        }

        // 🔹 Tạo Request (Dành cho Agency)
        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<ActionResult<RequestDto>> CreateRequest([FromBody] CreateRequestDto createRequestDto)
        {
            var userId = GetUserIdFromToken();
            var createdRequest = await _requestService.CreateRequestAsync(userId, createRequestDto);
            if (createdRequest == null)
                return BadRequest(new { message = "User is not an Agency." });

            return CreatedAtAction(nameof(GetRequestById), new { id = createdRequest.RequestId }, createdRequest);
        }

        // 🔹 Cập nhật Request (Chỉ cho phép chỉnh sửa `quantity` và `status`)
        [HttpPut("{id}")]
        [Authorize(Roles = "2")]
        public async Task<ActionResult<RequestDto>> UpdateRequest(long id, [FromBody] UpdateRequestDto updateRequestDto)
        {
            if (id != updateRequestDto.RequestId)
                return BadRequest(new { message = "Request ID does not match." });

            var updatedRequest = await _requestService.UpdateRequestAsync(updateRequestDto);
            if (updatedRequest == null) return NotFound(new { message = "Request not found." });

            return Ok(updatedRequest);
        }

        // 🔹 Phê duyệt Request (Chỉ dành cho Employee)
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "4")]
        public async Task<ActionResult> ApproveRequest(long id)
        {
            var userId = GetUserIdFromToken();
            var success = await _requestService.ApproveRequestAsync(id, userId);
            if (!success) return BadRequest(new { message = "Failed to approve request." });

            return Ok(new { message = "Request approved and converted to order." });
        }
    }
}
