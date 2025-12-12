using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public CategoriesController(ModiqueDbContext db)
        {
            _db = db;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _db.Categories.ToListAsync();
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // GET: api/categories/search?name=electronics
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search term cannot be empty.");

            var categories = await _db.Categories
                .Where(c => c.Name.Contains(name) || c.SubCategory.Contains(name))
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/5/products
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsByCategory(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            return Ok(category.Products ?? new List<Product>());
        }

        // GET: api/categories/subcategory/electronics
        [HttpGet("subcategory/{subcategory}")]
        public async Task<IActionResult> GetBySubCategory(string subcategory)
        {
            var categories = await _db.Categories
                .Where(c => c.SubCategory == subcategory)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Provjeri da li već postoji kategorija sa istim imenom
            var exists = await _db.Categories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

            if (exists)
                return BadRequest("Category with this name already exists.");

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
                return BadRequest("Category ID mismatch.");

            if (!await _db.Categories.AnyAsync(c => c.CategoryId == id))
                return NotFound();

            // Provjeri da li već postoji kategorija sa istim imenom (osim trenutne)
            var exists = await _db.Categories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower()
                          && c.CategoryId != id);

            if (exists)
                return BadRequest("Category with this name already exists.");

            _db.Entry(category).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            // Provjeri da li kategorija ima proizvode
            if (category.Products != null && category.Products.Any())
                return BadRequest("Cannot delete category with products. Remove products first or reassign them.");

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}