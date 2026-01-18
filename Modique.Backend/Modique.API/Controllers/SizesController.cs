using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Application.DTOs.Sizes;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SizesController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public SizesController(ModiqueDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sizes = await _db.Sizes.Select(s => new SizeDto
            {
                SizeId = s.SizeId,
                Code = s.Code,
                Description = s.Description
            }).ToListAsync();
            return Ok(sizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var size = await _db.Sizes.FirstOrDefaultAsync(s => s.SizeId == id);

            if (size == null)
                return NotFound();

            var sizeDto = new SizeDto
            {
                SizeId = size.SizeId,
                Code = size.Code,
                Description = size.Description
            };

            return Ok(sizeDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateSizeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _db.Sizes.AnyAsync(s => s.Code.ToLower() == dto.Code.ToLower());

            if (exists)
                return BadRequest(new { message = "Size with this code already exists." });

            var size = new Size
            {
                Code = dto.Code,
                Description = dto.Description
            };

            _db.Sizes.Add(size);
            await _db.SaveChangesAsync();

            var sizeDto = new SizeDto
            {
                SizeId = size.SizeId,
                Code = size.Code,
                Description = size.Description
            };

            return CreatedAtAction(nameof(GetById), new { id = size.SizeId }, sizeDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSizeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var size = await _db.Sizes.FirstOrDefaultAsync(s => s.SizeId == id);

            if (size == null)
                return NotFound();

            var exists = await _db.Sizes.AnyAsync(s => s.Code.ToLower() == dto.Code.ToLower() && s.SizeId != id);

            if (exists)
                return BadRequest(new { message = "Size with this code already exists." });

            size.Code = dto.Code;
            size.Description = dto.Description;

            await _db.SaveChangesAsync();

            var sizeDto = new SizeDto
            {
                SizeId = size.SizeId,
                Code = size.Code,
                Description = size.Description
            };

            return Ok(sizeDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var size = await _db.Sizes.FirstOrDefaultAsync(s => s.SizeId == id);

            if (size == null)
                return NotFound();

            var productOptions = await _db.ProductOptions.Where(po => po.SizeId == id).ToListAsync();

            if (productOptions.Any())
                return BadRequest(new { message = "Cannot delete size with product options. Remove product options first." });

            _db.Sizes.Remove(size);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
