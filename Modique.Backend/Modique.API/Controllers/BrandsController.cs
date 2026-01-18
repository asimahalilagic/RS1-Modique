using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Application.DTOs.Brands;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public BrandsController(ModiqueDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _db.Brands.Select(b => new BrandDto
            {
                BrandId = b.BrandId,
                Name = b.Name,
                Country = b.Country,
                LogoURL = b.LogoURL
            }).ToListAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.BrandId == id);

            if (brand == null)
                return NotFound();

            var brandDto = new BrandDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Country = brand.Country,
                LogoURL = brand.LogoURL
            };

            return Ok(brandDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _db.Brands.AnyAsync(b => b.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return BadRequest(new { message = "Brand with this name already exists." });

            var brand = new Brand
            {
                Name = dto.Name,
                Country = dto.Country,
                LogoURL = dto.LogoURL
            };

            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();

            var brandDto = new BrandDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Country = brand.Country,
                LogoURL = brand.LogoURL
            };

            return CreatedAtAction(nameof(GetById), new { id = brand.BrandId }, brandDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.BrandId == id);

            if (brand == null)
                return NotFound();

            var exists = await _db.Brands.AnyAsync(b => b.Name.ToLower() == dto.Name.ToLower() && b.BrandId != id);

            if (exists)
                return BadRequest(new { message = "Brand with this name already exists." });

            brand.Name = dto.Name;
            brand.Country = dto.Country;
            brand.LogoURL = dto.LogoURL;

            await _db.SaveChangesAsync();

            var brandDto = new BrandDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Country = brand.Country,
                LogoURL = brand.LogoURL
            };

            return Ok(brandDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _db.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.BrandId == id);

            if (brand == null)
                return NotFound();

            if (brand.Products != null && brand.Products.Any())
                return BadRequest(new { message = "Cannot delete brand with products. Remove products first." });

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
