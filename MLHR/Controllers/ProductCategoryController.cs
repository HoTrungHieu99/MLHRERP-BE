using System.Security.Claims;
using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _service;

        public ProductCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        [HttpGet("product-category")]
        [Authorize(Roles = "4,2")]
        public async Task<ActionResult<IEnumerable<ProductCategoryResponseDto>>> GetCategories()
        {
            return Ok(await _service.GetAllCategoriesAsync());
        }

        [HttpGet("product-category/{id}")]
        [Authorize(Roles = "4,2")]
        public async Task<ActionResult<ProductCategoryResponseDto>> GetCategory(long id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost("product-category")]
        [Authorize(Roles = "4")]
        public async Task<ActionResult<ProductCategoryResponseDto>> CreateCategory([FromBody] ProductCategoryDto categoryDto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "User ID is invalid." });
            }

            // Tạo danh mục sản phẩm
            var createdCategory = await _service.CreateCategoryAsync(categoryDto, userId);

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
        }

        [HttpPut("product-category{id}")]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> UpdateCategory(long id, [FromBody] ProductCategoryDto categoryDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "User ID is invalid." });
            }

            var updatedCategory = await _service.UpdateCategoryAsync(id, categoryDto, userId);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            return Ok(updatedCategory);
        }


        [HttpDelete("product-category{id}")]
        [Authorize(Roles = "4")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var result = await _service.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }


}
