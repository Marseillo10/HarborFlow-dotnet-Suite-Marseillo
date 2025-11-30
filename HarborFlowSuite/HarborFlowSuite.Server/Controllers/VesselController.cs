using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

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
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vessels = await _vesselService.GetVessels(firebaseUid);
            return Ok(vessels);
        }

        [HttpGet("positions")]
        public async Task<ActionResult<List<VesselPositionDto>>> GetVesselPositions()
        {
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var positions = await _vesselService.GetVesselPositions(firebaseUid);
            return Ok(positions);
        }

        [HttpGet("active")]
        public ActionResult<IEnumerable<VesselPositionUpdateDto>> GetActiveVessels()
        {
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vessels = _vesselService.GetActiveVessels(firebaseUid);
            return Ok(vessels);
        }

        [HttpGet("positions/{mmsi}")]
        public async Task<ActionResult<VesselPositionDto>> GetVesselPosition(string mmsi, [FromQuery] bool allowGfwFallback = true)
        {
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var position = await _vesselService.GetVesselPosition(mmsi, firebaseUid, allowGfwFallback);
            if (position == null)
            {
                return NotFound();
            }
            return Ok(position);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vessel>> GetVessel(Guid id)
        {
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vessel = await _vesselService.GetVesselById(id, firebaseUid);
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
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vessels = await _vesselService.GetVessels(firebaseUid);
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