using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới (User)
        /// </summary>
        /// <param name="request">Thông tin người dùng</param>
        /// <returns>Trả về UserId nếu đăng ký thành công</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _userService.RegisterAsync(request);
                return Ok(new { message = "Registration successful, please wait for admin approval!", userId = user.UserId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Duyệt tài khoản: Cập nhật trạng thái và lưu vào Employee hoặc AgencyAccount
        /// </summary>
        /// <param name="userId">ID của tài khoản cần duyệt</param>
        /// <returns>Trả về thông báo thành công hoặc lỗi</returns>
        [HttpPut("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(long userId)
        {
            try
            {
                bool isApproved = await _userService.ApproveUserAsync(userId);

                if (!isApproved)
                    return BadRequest(new { message = "Cannot update user status or User does not exist!" });

                return Ok(new { message = "Account has been approved and saved to the respective table!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
