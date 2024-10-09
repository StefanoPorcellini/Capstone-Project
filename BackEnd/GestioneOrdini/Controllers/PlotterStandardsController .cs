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
    [Authorize(Roles = "Admin, Titolare")]
    public class PlotterStandardsController : ControllerBase
    {
        private readonly IGenericService<PlotterStandard> _plotterStandardService;

        public PlotterStandardsController(IGenericService<PlotterStandard> plotterStandardService)
        {
            _plotterStandardService = plotterStandardService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<PlotterStandard>> CreatePlotterStandard(PlotterStandard plotterStandard)
        {
            var createdPlotterStandard = await _plotterStandardService.CreateAsync(plotterStandard);
            return CreatedAtAction(nameof(GetPlotterStandardById), new { id = createdPlotterStandard.Id }, createdPlotterStandard);
        }

        [HttpGet("getByid/{id}")]
        [Authorize]
        public async Task<ActionResult<PlotterStandard>> GetPlotterStandardById(int id)
        {
            var plotterStandard = await _plotterStandardService.GetByIdAsync(id);
            if (plotterStandard == null)
            {
                return NotFound();
            }
            return Ok(plotterStandard);
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlotterStandard>>> GetAllPlotterStandards()
        {
            var plotterStandards = await _plotterStandardService.GetAllAsync();
            return Ok(plotterStandards);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePlotterStandard(int id, PlotterStandard plotterStandard)
        {
            if (id != plotterStandard.Id)
            {
                return BadRequest();
            }

            await _plotterStandardService.UpdateAsync(plotterStandard);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePlotterStandard(int id)
        {
            await _plotterStandardService.DeleteAsync(id);
            return NoContent();
        }
    }
}
