using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Application.DTOs.Colors;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public ColorsController(ModiqueDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _db.Colors.Select(c => new ColorDto
            {
                ColorId = c.ColorId,
                Name = c.Name,
                ColorCode = c.ColorCode
            }).ToListAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _db.Colors.FirstOrDefaultAsync(c => c.ColorId == id);

            if (color == null)
                return NotFound();

            var colorDto = new ColorDto
            {
                ColorId = color.ColorId,
                Name = color.Name,
                ColorCode = color.ColorCode
            };

            return Ok(colorDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateColorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _db.Colors.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return BadRequest(new { message = "Color with this name already exists." });

            var color = new Color
            {
                Name = dto.Name,
                ColorCode = dto.ColorCode
            };

            _db.Colors.Add(color);
            await _db.SaveChangesAsync();

            var colorDto = new ColorDto
            {
                ColorId = color.ColorId,
                Name = color.Name,
                ColorCode = color.ColorCode
            };

            return CreatedAtAction(nameof(GetById), new { id = color.ColorId }, colorDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateColorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var color = await _db.Colors.FirstOrDefaultAsync(c => c.ColorId == id);

            if (color == null)
                return NotFound();

            var exists = await _db.Colors.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.ColorId != id);

            if (exists)
                return BadRequest(new { message = "Color with this name already exists." });

            color.Name = dto.Name;
            color.ColorCode = dto.ColorCode;

            await _db.SaveChangesAsync();

            var colorDto = new ColorDto
            {
                ColorId = color.ColorId,
                Name = color.Name,
                ColorCode = color.ColorCode
            };

            return Ok(colorDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var color = await _db.Colors
                .Include(c => c.ProductOptions)
                .FirstOrDefaultAsync(c => c.ColorId == id);

            if (color == null)
                return NotFound();

            if (color.ProductOptions != null && color.ProductOptions.Any())
                return BadRequest(new { message = "Cannot delete color with product options. Remove product options first." });

            _db.Colors.Remove(color);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
