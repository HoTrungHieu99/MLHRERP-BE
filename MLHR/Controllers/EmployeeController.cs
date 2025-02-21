using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController( IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách Employee</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetEmployee();

                if (employees == null || employees.Count == 0)
                    return NotFound(new { message = "There are no employees in the system!" });

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin Employee
        /// </summary>
        [HttpPut("update-employee/{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(long employeeId, [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                bool isUpdated = await _employeeService.UpdateEmployeeAsync(employeeId, request);

                if (!isUpdated)
                    return BadRequest(new { message = "Unable to update Employee information!" });

                return Ok(new { message = "Employee information updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
