using BusinessObject.Models;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy tất cả User
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }


        /// <summary>
        /// Lấy thông tin User theo ID.
        /// </summary>
        /// <param name="id">ID của User</param>
        /// <returns>Thông tin User</returns>
        /// <response code="200">Thành công</response>
        /// <response code="404">Không tìm thấy User</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public IActionResult GetUserById(long id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = $"ID {id} does not exist!",
                    Instance = HttpContext.Request.Path
                });
            }
            return Ok(user);
        }

        /// <summary>
        /// Thêm User mới vào hệ thống.
        /// </summary>
        /// <param name="user">Thông tin User cần thêm</param>
        /// <returns>Trả về thông tin User sau khi thêm</returns>
        /// <response code="201">Tạo mới thành công</response>
        /// <response code="400">User_Type không hợp lệ</response>
        /// <response code="409">UserName, Email hoặc Password đã tồn tại</response>
        [HttpPost("AddUser")]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (InvalidOperationException ex) // Xử lý lỗi trùng UserName, Email, Password
            {
                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Conflict",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (ArgumentException ex) // Xử lý lỗi User_Type không hợp lệ
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Bad Request",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public IActionResult UpdateUser(long id, [FromBody] User user)
        {
            try
            {
                if (id != user.UserId)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Bad Request",
                        Detail = "User ID in URL and body do not match!",
                        Instance = HttpContext.Request.Path
                    });
                }

                var updatedUser = _userService.UpdateUser(user);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex) // Xử lý lỗi nếu không tìm thấy User
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (InvalidOperationException ex) // Xử lý lỗi nếu Username, Email trùng hoặc Password trùng với cũ
            {
                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Conflict",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
        }


        [HttpDelete("DeleteUser/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public IActionResult DeleteUser(long id)
        {
            try
            {
                _userService.DeleteUser(id);
                return NoContent(); // Trả về 204 No Content nếu xóa thành công
            }
            catch (KeyNotFoundException ex) // Xử lý lỗi nếu không tìm thấy User
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                });
            }
        }
    }
}
