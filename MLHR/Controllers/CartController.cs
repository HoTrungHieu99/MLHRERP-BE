using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MLHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController()
        {
            _cartService = new CartService();
        }

        [HttpGet("available-products")]
        public IActionResult GetAvailableProducts()
        {
            var products = _cartService.GetAvailableProducts();
            return Ok(products);
        }

        [HttpPost("add-from-stock")]
        public IActionResult AddProductToCart([FromQuery] string productId, [FromQuery] int quantity)
        {
            var result = _cartService.AddProductToCart(productId, quantity);
            return Ok(new { Message = result });
        }

        [HttpGet("cart")]
        public IActionResult GetCartItems()
        {
            var cartItems = _cartService.GetCartItems();
            var totalPrice = _cartService.GetCartTotalPrice();

            return Ok(new
            {
                Items = cartItems,
                TotalPrice = totalPrice
            });
        }
    }
}
