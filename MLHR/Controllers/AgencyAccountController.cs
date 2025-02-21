using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.IService;
using Services.Service;

namespace MLHR.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AgencyAccountController : Controller
    {
        private readonly IAgencyAccountService _agencyAccount;

        public AgencyAccountController(IAgencyAccountService agencyAccount)
        {
            _agencyAccount = agencyAccount;
        }
        /// <summary>
        /// Lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách Employee</returns>
        [HttpGet("get-all-agencyaccount")]
        public async Task<IActionResult> GetAllAgencyAccount()
        {
            try
            {
                var agencyAccounts = await _agencyAccount.GetAgencyAccount();

                if (agencyAccounts == null || agencyAccounts.Count == 0)
                    return NotFound(new { message = "There are no agency account in the system!" });

                return Ok(agencyAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin Employee theo UserId
        /// </summary>
        /// <param name="userId">ID của User</param>
        /// <param name="request">Thông tin mới của Employee</param>
        /// <returns>Thông báo thành công hoặc lỗi</returns>
        [HttpPut("update-agencyaccount/{agencyId}")]
        public async Task<IActionResult> UpdateEmployee(int agencyId, [FromBody] AgencyAccountRequest request)
        {
            try
            {
                bool isUpdated = await _agencyAccount.UpdateAgencyAccountAsync(agencyId, request);

                if (!isUpdated)
                    return BadRequest(new { message = "Unable to update agency account information!" });

                return Ok(new { message = "Agency account information updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
