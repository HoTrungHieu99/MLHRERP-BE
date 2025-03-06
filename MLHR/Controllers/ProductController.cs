﻿using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles = "4")] // Yêu cầu xác thực JWT
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpGet("product")]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _service.GetProductsAsync(page, pageSize);

            if (products == null || products.Items.Count == 0)
            {
                return NotFound(new { message = "Không có dữ liệu." });
            }

            return Ok(products);
        }

        [HttpGet("product/{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(long id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("product")]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] ProductDto productDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "User ID is invalid." });
            }

            var createdProduct = await _service.CreateProductAsync(productDto, userId);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut("product/{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductDto productDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "User ID is invalid." });
            }

            var updatedProduct = await _service.UpdateProductAsync(id, productDto, userId);
            if (updatedProduct == null) return NotFound();

            return Ok(updatedProduct);
        }

        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var result = await _service.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // ✅ API cập nhật AvailableStock
        [HttpPut("update-available-stock")]
        public async Task<IActionResult> UpdateAvailableStock()
        {
            await _service.UpdateAvailableStockAsync();
            return Ok(new { message = "AvailableStock updated successfully!" });
        }
    }
}
