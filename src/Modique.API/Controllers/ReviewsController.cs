using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public ReviewsController(ModiqueDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Reviews.Include(r => r.Product).Include(r => r.User).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = review.ReviewId }, review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Review review)
        {
            if (id != review.ReviewId) return BadRequest("Review ID mismatch.");
            if (!await _db.Reviews.AnyAsync(r => r.ReviewId == id)) return NotFound();

            _db.Entry(review).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
