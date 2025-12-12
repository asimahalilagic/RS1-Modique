using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public CartController(ModiqueDbContext db)
        {
            _db = db;
        }

        // GET: api/cart/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Kreiraj novu korpu ako ne postoji
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<CartItem>()
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            return Ok(cart);
        }

        // GET: api/cart/5
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Provjeri da li proizvod postoji
            var product = await _db.Products.FindAsync(request.ProductId);
            if (product == null)
                return NotFound("Product not found.");

            // Provjeri da li korisnik ima korpu
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            // Kreiraj novu korpu ako ne postoji
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            // Provjeri da li proizvod već postoji u korpi
            var existingItem = cart.Items?.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Ažuriraj količinu
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // Dodaj novi item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = product.Price
                };
                _db.CartItems.Add(cartItem);
            }

            await _db.SaveChangesAsync();

            // Vrati ažuriranu korpu
            var updatedCart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cart.CartId);

            return Ok(updatedCart);
        }

        // PUT: api/cart/update-quantity
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cartItem = await _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == request.CartItemId);

            if (cartItem == null)
                return NotFound("Cart item not found.");

            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            cartItem.Quantity = request.Quantity;
            await _db.SaveChangesAsync();

            // Vrati ažuriranu korpu
            var updatedCart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

            return Ok(updatedCart);
        }

        // DELETE: api/cart/remove/5
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cartItem = await _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
                return NotFound("Cart item not found.");

            var cartId = cartItem.CartId;
            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();

            // Vrati ažuriranu korpu
            var updatedCart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            return Ok(updatedCart);
        }

        // DELETE: api/cart/clear/5
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Cart not found.");

            _db.CartItems.RemoveRange(cart.Items);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cart cleared successfully." });
        }

        // GET: api/cart/user/5/total
        [HttpGet("user/{userId}/total")]
        public async Task<IActionResult> GetCartTotal(int userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return Ok(new { total = 0, itemCount = 0 });

            var total = cart.Items?.Sum(i => i.Quantity * i.Price) ?? 0;
            var itemCount = cart.Items?.Sum(i => i.Quantity) ?? 0;

            return Ok(new { total, itemCount });
        }
    }

    // DTO classes
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateQuantityRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}