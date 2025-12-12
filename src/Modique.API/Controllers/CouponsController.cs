using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public CouponsController(ModiqueDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Coupons.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Coupon coupon)
        {
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = coupon.CouponId }, coupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Coupon coupon)
        {
            if (id != coupon.CouponId) return BadRequest("Coupon ID mismatch.");
            if (!await _db.Coupons.AnyAsync(c => c.CouponId == id)) return NotFound();

            _db.Entry(coupon).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(coupon);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            _db.Coupons.Remove(coupon);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
