﻿using BusinessObject.DTO.AgencyLevel;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IService;

namespace MLHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "4")]
    public class AgencyLevelController : ControllerBase
    {
        private readonly IAgencyLevelService _service;

        public AgencyLevelController(IAgencyLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var levels = await _service.GetAllLevelsAsync();
            return Ok(levels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var level = await _service.GetLevelByIdAsync(id);
            if (level == null) return NotFound();
            return Ok(level);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAgencyLevelDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.CreateLevelAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateAgencyLevelDto dto)
        {
            await _service.UpdateLevelAsync(id, dto);
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteLevelAsync(id);
            return NoContent();
        }
    }
}
