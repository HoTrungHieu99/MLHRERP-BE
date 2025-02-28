using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet("role")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return Ok(await _service.GetAllRolesAsync());
        }
    }
}
