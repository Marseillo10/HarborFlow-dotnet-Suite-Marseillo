using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HarborFlowSuite.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MapBookmarkController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MapBookmarkController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapBookmark>>> GetMapBookmarks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }
        return await _context.MapBookmarks.Where(mb => mb.UserId == userId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<MapBookmark>> PostMapBookmark(CreateMapBookmarkDto createMapBookmarkDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var mapBookmark = new MapBookmark
        {
            Id = Guid.NewGuid(),
            Name = createMapBookmarkDto.Name,
            Latitude = (decimal)createMapBookmarkDto.Latitude,
            Longitude = (decimal)createMapBookmarkDto.Longitude,
            UserId = userId
        };

        _context.MapBookmarks.Add(mapBookmark);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMapBookmark), new { id = mapBookmark.Id }, mapBookmark);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MapBookmark>> GetMapBookmark(Guid id)
    {
        var mapBookmark = await _context.MapBookmarks.FindAsync(id);

        if (mapBookmark == null)
        {
            return NotFound();
        }

        return mapBookmark;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMapBookmark(Guid id, MapBookmark mapBookmark)
    {
        if (id != mapBookmark.Id)
        {
            return BadRequest();
        }

        _context.Entry(mapBookmark).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MapBookmarkExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMapBookmark(Guid id)
    {
        var mapBookmark = await _context.MapBookmarks.FindAsync(id);
        if (mapBookmark == null)
        {
            return NotFound();
        }

        _context.MapBookmarks.Remove(mapBookmark);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MapBookmarkExists(Guid id)
    {
        return _context.MapBookmarks.Any(e => e.Id == id);
    }
}
