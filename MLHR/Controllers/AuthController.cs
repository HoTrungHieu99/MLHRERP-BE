using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;
using System.Security.Claims;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Đăng nhập bằng Email & Password
        /// </summary>
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _userService.LoginAsync(request);
                return Ok(new { token });
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                bool isLoggedOut = await _userService.LogoutAsync(request.Email);
                if (!isLoggedOut)
                {
                    return BadRequest(new { message = "Logout failed!" });
                }

                return Ok(new { message = "Logout successful!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("user/{id?}")] // ✅ `id` là tùy chọn (nullable)
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // 🔹 Lấy UserId & Role từ Token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }

                Guid userId = Guid.Parse(userIdClaim); // ✅ Chuyển đổi UserId từ Token
                int role = int.Parse(roleClaim); // ✅ Chuyển Role từ Token

                // ✅ Nếu `id` là "me" hoặc rỗng, tự động lấy UserId từ Token
                if (string.IsNullOrEmpty(id) || id.ToLower() == "me")
                {
                    id = userId.ToString(); // Tự động gán UserId từ Token
                }

                // ✅ Chuyển đổi `id` sang GUID (nếu không phải "me")
                if (!Guid.TryParse(id, out Guid targetUserId))
                {
                    return BadRequest(new { message = "Invalid User ID format." });
                }

                // ✅ Nếu không phải Admin và User cố cập nhật người khác -> Lỗi
                if (role != 1 && targetUserId != userId)
                {
                    return Unauthorized(new { message = "You can only update your own account." });
                }

                // 🔹 Gọi Service để cập nhật User
                bool isUpdated = await _userService.UpdateUserAccountAsync(targetUserId, request);
                if (!isUpdated)
                    return BadRequest(new { message = "Update failed!" });

                return Ok(new { message = "User account updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        /*[HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                bool isSuccessful = await _userService.ForgotPasswordAsync(request);
                if (!isSuccessful)
                    return BadRequest(new { message = "Failed to reset password!" });

                return Ok(new { message = "A new password has been sent to your email!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }*/

        [HttpPost("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                bool isSuccessful = await _userService.ChangePasswordAsync(userId, request);
                if (!isSuccessful)
                    return BadRequest(new { message = "Failed to change password!" });

                return Ok(new { message = "Password changed successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpPut("change-employee-role/{userId}")]
        public async Task<IActionResult> ChangeEmployeeRole(Guid userId)
        {
            try
            {
                bool isSuccessful = await _userService.ChangeEmployeeRoleAsync(userId);
                if (!isSuccessful)
                    return BadRequest(new { message = "Failed to change role!" });

                return Ok(new { message = "Employee role changed successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var users = await _userService.GetUsersAsync(page, pageSize);

            if (users == null || users.Items.Count == 0)
            {
                return NotFound(new { message = "Không có dữ liệu." });
            }

            return Ok(users);
        }
    }
}
