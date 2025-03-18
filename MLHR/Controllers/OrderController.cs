using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
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

        // API thanh toán đơn hàng
        [HttpPut("{orderId}/payment")]
        public async Task<IActionResult> ProcessPayment(Guid orderId)
        {
            var result = await _orderService.ProcessPaymentAsync(orderId);
            if (!result) return BadRequest("Thanh toán thất bại hoặc đơn hàng không hợp lệ.");
            return Ok("Thanh toán thành công.");
        }

        // ✅ API để hủy đơn hàng
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(orderId);
                if (!result) return BadRequest("Không thể hủy đơn hàng! Đơn hàng không tồn tại hoặc đã được xử lý.");
                return Ok("Đơn hàng đã được hủy thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
