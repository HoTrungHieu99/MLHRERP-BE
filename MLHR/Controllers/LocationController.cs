using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _provinceService;

        public LocationController(ILocationService provinceService)
        {
            _provinceService = provinceService;
        }

        // 🔹 API Lấy danh sách tỉnh/thành
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _provinceService.GetProvincesAsync();
            return Ok(provinces);
        }

        // 🔹 API Lấy danh sách huyện/quận theo province_id
        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistrict(int provinceId)
        {
            var district = await _provinceService.GetDistrictAsync(provinceId);

            if (district == null)
            {
                return NotFound($"Không có dữ liệu cho districtId = {provinceId}.");
            }

            return Ok(district);
        }

        // 🔹 API Lấy danh sách xã/phường theo district_id
        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var wards = await _provinceService.GetWardsAsync(districtId);
            return Ok(wards);
        }

        /*[HttpPost("import-all")]
        public async Task<IActionResult> ImportAllLocations()
        {
            try
            {
                await _provinceService.ImportAllLocations();
                return Ok("Nhập dữ liệu tỉnh, huyện, xã thành công!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi trong quá trình nhập dữ liệu: {ex.Message}");
            }
        }*/

    }
}
