using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Người dùng gửi yêu cầu đăng ký
        /// </summary>
        [HttpPost("auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var registerAccount = await _userService.RegisterUserRequestAsync(request);
                return Ok(new { message = "Registration successful, please wait for admin approval!", registerId = registerAccount.RegisterId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        /// <summary>
        /// Admin duyệt tài khoản đăng ký
        /// </summary>
        [HttpPut("auth/register/{id}")]
        public async Task<IActionResult> ApproveUser(int registerId)
        {
            try
            {
                bool isApproved = await _userService.ApproveUserAsync(registerId);
                if (!isApproved)
                    return BadRequest(new { message = "Account cannot be approved or account does not exist!" });

                return Ok(new { message = "Account approved successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
