using BusinessObject.DTO.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles = "4")] // Yêu cầu xác thực bằng JWT
    public class TaxConfigController : ControllerBase
    {
        private readonly ITaxConfigService _taxConfigService;

        public TaxConfigController(ITaxConfigService taxConfigService)
        {
            _taxConfigService = taxConfigService;
        }

        [HttpGet("taxconfig")]
        public async Task<ActionResult<IEnumerable<TaxConfigDTO>>> GetAll()
        {
            var result = await _taxConfigService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("tax-config/{id}")]
        public async Task<ActionResult<TaxConfigDTO>> GetById(int id)
        {
            var result = await _taxConfigService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("tax-config")]
        public async Task<ActionResult<TaxConfigDTO>> Create(TaxConfigDTO taxConfigDto)
        {
            var result = await _taxConfigService.AddAsync(taxConfigDto);
            return CreatedAtAction(nameof(GetById), new { id = result.TaxId }, result);
        }

        [HttpPatch("tax-config/{id}")]
        public async Task<ActionResult<TaxConfigDTO>> Update(int id, [FromBody] TaxConfigDTO taxConfigDto)
        {
            if (id != taxConfigDto.TaxId)
                return BadRequest("ID in path does not match ID in body.");

            var result = await _taxConfigService.UpdateAsync(taxConfigDto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("tax-config/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _taxConfigService.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
