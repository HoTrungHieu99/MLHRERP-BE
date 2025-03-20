using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public OrderController(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ API để lấy danh sách Order (Bao gồm chi tiết đơn hàng)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders.Select(o => new
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity), // Tính tổng tiền
                Status = o.Status,
                OrderDetails = o.OrderDetails.Select(od => new
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.UnitPrice,
                    SubTotal = od.Quantity * od.UnitPrice
                })
            }));
        }

        // API để lấy chi tiết một Order
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng!");
            return Ok(new
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity), // Tính tổng tiền
                Status = order.Status,
                OrderDetails = order.OrderDetails.Select(od => new
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.UnitPrice,
                    SubTotal = od.Quantity * od.UnitPrice
                })
            });
        }

        // API lấy danh sách Order dựa trên AgencyId của user đang đăng nhập
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetOrdersForLoggedInAgency()
        {
            var agencyId = GetLoggedInAgencyId();
            if (agencyId == null)
            {
                return Unauthorized(new { message = "User is not associated with any agency" });
            }

            var orders = await _orderService.GetOrdersByAgencyIdAsync(agencyId.Value);

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this agency" });
            }

            return Ok(orders);
        }

        private long? GetLoggedInAgencyId()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var agencyIdClaim = claimsIdentity.FindFirst("AgencyId");
                if (agencyIdClaim != null && long.TryParse(agencyIdClaim.Value, out long agencyId))
                {
                    return agencyId;
                }
            }
            return null;
        }

        // API thanh toán đơn hàng
        [HttpPut("{orderId}/payment")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> ProcessPayment(Guid orderId)
        {
            try
            {
                var result = await _orderService.ProcessPaymentAsync(orderId);
                if (result)
                    return Ok(new { message = "Payment processed successfully!" });

                return BadRequest(new { message = "Failed to process payment." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelRequest(Guid orderId)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Failed to cancel request." });

                return Ok(new { message = "Order canceled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
