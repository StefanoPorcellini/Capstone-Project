using GestioneOrdini.Interface;
using GestioneOrdini.Model.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin, Titolare")]
    public class LaserStandardsController : ControllerBase
    {
        private readonly IGenericService<LaserStandard> _laserStandardService;

        public LaserStandardsController(IGenericService<LaserStandard> laserStandardService)
        {
            _laserStandardService = laserStandardService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<LaserStandard>> CreateLaserStandard(LaserStandard laserStandard)
        {
            var createdLaserStandard = await _laserStandardService.CreateAsync(laserStandard);
            return CreatedAtAction(nameof(GetLaserStandardById), new { id = createdLaserStandard.Id }, createdLaserStandard);
        }

        [HttpGet("getById/{id}")]
        [Authorize]
        public async Task<ActionResult<LaserStandard>> GetLaserStandardById(int id)
        {
            var laserStandard = await _laserStandardService.GetByIdAsync(id);
            if (laserStandard == null)
            {
                return NotFound();
            }
            return Ok(laserStandard);
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LaserStandard>>> GetAllLaserStandards()
        {
            var laserStandards = await _laserStandardService.GetAllAsync();
            return Ok(laserStandards);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLaserStandard(int id, LaserStandard laserStandard)
        {
            if (id != laserStandard.Id)
            {
                return BadRequest();
            }

            await _laserStandardService.UpdateAsync(laserStandard);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLaserStandard(int id)
        {
            await _laserStandardService.DeleteAsync(id);
            return NoContent();
        }
    }
}
