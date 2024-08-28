using GestioneOrdini.Interface;
using GestioneOrdini.Model.PriceList;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaserPriceListController : ControllerBase
    {
        private readonly ILaserPriceListService _laserPriceListService;

        public LaserPriceListController(ILaserPriceListService laserPriceListService)
        {
            _laserPriceListService = laserPriceListService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLaserPriceList([FromBody] LaserPriceList priceList)
        {
            if (ModelState.IsValid)
            {
                var createdPriceList = await _laserPriceListService.CreateLaserPriceListAsync(priceList);
                return CreatedAtAction(nameof(GetLaserPriceListById), new { id = createdPriceList.Id }, createdPriceList);
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLaserPriceLists()
        {
            var priceLists = await _laserPriceListService.GetAllLaserPriceListsAsync();
            return Ok(priceLists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLaserPriceListById(int id)
        {
            var priceList = await _laserPriceListService.GetLaserPriceListByIdAsync(id);
            if (priceList != null)
            {
                return Ok(priceList);
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLaserPriceList(int id, [FromBody] LaserPriceList priceList)
        {
            if (id != priceList.Id)
            {
                return BadRequest("ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                await _laserPriceListService.UpdateLaserPriceListAsync(priceList);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLaserPriceList(int id)
        {
            await _laserPriceListService.DeleteLaserPriceListAsync(id);
            return NoContent();
        }
    }
}
