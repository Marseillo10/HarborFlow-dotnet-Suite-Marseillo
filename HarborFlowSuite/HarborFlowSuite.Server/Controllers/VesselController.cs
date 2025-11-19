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
    }
}