using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestExportController : ControllerBase
    {
        private readonly IRequestExportService _requestExportService;

        public RequestExportController(IRequestExportService requestExportService)
        {
            _requestExportService = requestExportService;
        }

        // ✅ API GET: Lấy danh sách RequestExport kèm RequestExportDetail
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequestExports()
        {
            var requestExports = await _requestExportService.GetAllRequestExportsAsync();
            return Ok(requestExports);
        }
    }
}
