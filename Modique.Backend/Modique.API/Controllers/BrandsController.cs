using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: api/brands
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _db.Brands.ToListAsync();
            return Ok(brands);
        }

        // GET: api/brands/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _db.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.BrandId == id);

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }
    }
}
