using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/request-products")]
    [ApiController]
    public class RequestProductController : ControllerBase
    {
        private readonly IRequestProductService _requestProductService;
        private readonly IUserService _userService;

        public RequestProductController(IRequestProductService requestProductService, IUserService userService)
        {
            _requestProductService = requestProductService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _requestProductService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestProductDto requestDto)
        {
            if (requestDto == null || requestDto.Products == null || requestDto.Products.Count == 0)
            {
                return BadRequest("Request must contain at least one product.");
            }

            // **Lấy UserId từ token dưới dạng GUID**
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // **Gọi Service để lấy AgencyId từ UserId**
            var agencyId = await _userService.GetAgencyIdByUserId(userId);
            if (!agencyId.HasValue)
            {
                return BadRequest("User does not belong to any agency.");
            }

            var requestProduct = new RequestProduct
            {
                AgencyId = agencyId.Value, // ✅ Chuyển `long?` thành `long`
                RequestStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var requestDetails = new List<RequestProductDetail>();
            foreach (var item in requestDto.Products)
            {
                requestDetails.Add(new RequestProductDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            await _requestProductService.CreateRequestAsync(requestProduct, requestDetails);
            return Ok(requestProduct);
        }

        [Authorize(Roles = "4")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            try
            {
                // **Lấy UserId từ token dưới dạng GUID**
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // **Gọi Service để lấy AgencyId từ UserId**
                var employeeId = await _userService.GetEmployeeIdByUserId(userId);
                if (!employeeId.HasValue)
                {
                    return BadRequest("User does not belong to any employee.");
                }

                // ✅ Ép kiểu `agencyId` từ `long?` thành `long`
                await _requestProductService.ApproveRequestAsync(id, employeeId.Value);

                return Ok(new { message = "Request approved and Order created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
