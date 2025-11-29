using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VesselController : ControllerBase
    {
        private readonly IVesselService _vesselService;

        public VesselController(IVesselService vesselService)
        {
            _vesselService = vesselService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Vessel>>> GetVessels()
        {
            var vessels = await _vesselService.GetVessels();
            return Ok(vessels);
        }

        [HttpGet("positions")]
        public async Task<ActionResult<List<VesselPositionDto>>> GetVesselPositions()
        {
            var positions = await _vesselService.GetVesselPositions();
            return Ok(positions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vessel>> GetVessel(Guid id)
        {
            var vessel = await _vesselService.GetVesselById(id);
            if (vessel == null)
            {
                return NotFound();
            }
            return Ok(vessel);
        }

        [HttpPost]
        public async Task<ActionResult<Vessel>> CreateVessel([FromBody] Vessel vessel)
        {
            var createdVessel = await _vesselService.CreateVessel(vessel);
            return CreatedAtAction(nameof(GetVessels), new { id = createdVessel.Id }, createdVessel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVessel(Guid id, [FromBody] Vessel vessel)
        {
            if (id != vessel.Id)
            {
                return BadRequest();
            }

            var updatedVessel = await _vesselService.UpdateVessel(vessel);
            if (updatedVessel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVessel(Guid id)
        {
            var result = await _vesselService.DeleteVessel(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var vessels = await _vesselService.GetVessels();
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Id,Name,MMSI,ImoNumber,VesselType,Length,Width,IsActive");

            foreach (var v in vessels)
            {
                builder.AppendLine($"{v.Id},{EscapeCsv(v.Name)},{v.MMSI},{v.ImoNumber},{v.VesselType},{v.Length},{v.Width},{v.IsActive}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "vessels.csv");
        }

        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}