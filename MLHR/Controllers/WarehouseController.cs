using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Chỉ người dùng đăng nhập mới có quyền truy cập
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // Tạo Warehouse (chỉ cho Warehouse Manager)
        [Authorize(Roles = "3")]
        [HttpPost]
        public IActionResult CreateWarehouse([FromBody] WarehouseCreateRequest request)
        {
            var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            if (roleId != 3)
                return Unauthorized("Chỉ Warehouse Manager (RoleId = 3) mới có quyền.");

            _warehouseService.CreateWarehouse(userId, request.WarehousName, request.Street, request.Province, request.District, request.Ward);

            return Ok("Warehouse đã được tạo.");
        }

        // Lấy Warehouse của Employee hiện tại
        [Authorize(Roles = "3")]
        [HttpGet("Get-Warehouse")]
        public IActionResult GetMyWarehouse()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var warehouse = _warehouseService.GetWarehouseByUserId(userId);

            if (warehouse == null)
                return NotFound("You do not have a Warehouse!");

            return Ok(warehouse);
        }

        // Xóa Warehouse (chỉ Warehouse Manager)
        [Authorize(Roles = "3")]
        [HttpPut("upate-warehouse/{warehouseId}")]
        public IActionResult UpdateWarehouse(int warehouseId, [FromBody] WarehouseUpdateRequest request)
        {
            var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            if (roleId != 3)
                return Unauthorized("Chỉ Warehouse Manager (RoleId = 3) mới có quyền.");

            try
            {
                _warehouseService.UpdateWarehouse(userId, warehouseId, request.WarehousName, request.Street, request.Province, request.District, request.Ward);
                return Ok("Warehouse đã được cập nhật.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       /* // Xóa Warehouse (chỉ Warehouse Manager)
        [Authorize(Roles = "3")]
        [HttpDelete("delete-warehouse/{warehouseId}")]
        public IActionResult DeleteWarehouse(int warehouseId)
        {
            var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");
            if (roleId != 3)
                return Unauthorized("Only Warehouse Manager (RoleId = 3) has this permission!");

            _warehouseService.DeleteWarehouse(warehouseId);
            return Ok("Warehouse has been deleted!");
        }*/
    }
}
