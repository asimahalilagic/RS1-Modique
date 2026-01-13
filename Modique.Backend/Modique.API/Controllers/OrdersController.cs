using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ModiqueDbContext _db;

        public OrdersController(ModiqueDbContext db)
        {
            _db = db;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductOption)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductOption)
                .Include(o => o.ReturnRequests)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // GET: api/orders/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var orders = await _db.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductOption)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/number/ORD-2024-001
        [HttpGet("number/{orderNumber}")]
        public async Task<IActionResult> GetByOrderNumber(string orderNumber)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductOption)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // GET: api/orders/status/2
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetByStatus(int statusId)
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.Items)
                .Where(o => o.OrderStatusId == statusId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Provjeri da li korisnik postoji
            var userExists = await _db.Users.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
                return NotFound("User not found.");

            // Provjeri da li adresa postoji
            var addressExists = await _db.Addresses.AnyAsync(a => a.AddressId == request.AddressId);
            if (!addressExists)
                return NotFound("Address not found.");

            // Provjeri da li status postoji
            var statusExists = await _db.OrderStatuses.AnyAsync(s => s.OrderStatusId == request.OrderStatusId);
            if (!statusExists)
                return NotFound("Order status not found.");

            // Provjeri da li shipping method postoji
            var shippingExists = await _db.ShippingMethods.AnyAsync(s => s.ShippingMethodId == request.ShippingMethodId);
            if (!shippingExists)
                return NotFound("Shipping method not found.");

            // Provjeri da li payment method postoji
            var paymentExists = await _db.PaymentMethods.AnyAsync(p => p.PaymentMethodId == request.PaymentMethodId);
            if (!paymentExists)
                return NotFound("Payment method not found.");

            // Generiši jedinstveni order number
            var orderNumber = GenerateOrderNumber();

            // Kreiraj narudžbinu
            var order = new Order
            {
                UserId = request.UserId,
                AddressId = request.AddressId,
                OrderStatusId = request.OrderStatusId,
                ShippingMethodId = request.ShippingMethodId,
                PaymentMethodId = request.PaymentMethodId,
                CreatedAt = DateTime.UtcNow,
                OrderNumber = orderNumber,
                Total = 0 // Biće kalkulisano nakon dodavanja items
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Dodaj items iz requesta
            decimal total = 0;
            foreach (var item in request.Items)
            {
                var productOption = await _db.ProductOptions.FindAsync(item.ProductOptionId);
                if (productOption == null)
                    continue;

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductOptionId = item.ProductOptionId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _db.OrderItems.Add(orderItem);
                total += item.Price * item.Quantity;
            }

            // Ažuriraj total
            order.Total = total;
            await _db.SaveChangesAsync();

            // Opciono: Obriši korpu korisnika nakon uspješne narudžbine
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart != null)
            {
                _db.CartItems.RemoveRange(cart.Items);
                await _db.SaveChangesAsync();
            }

            // Vrati kreiranu narudžbinu
            var createdOrder = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductOption)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, createdOrder);
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var statusExists = await _db.OrderStatuses.AnyAsync(s => s.OrderStatusId == request.OrderStatusId);
            if (!statusExists)
                return NotFound("Order status not found.");

            order.OrderStatusId = request.OrderStatusId;
            await _db.SaveChangesAsync();

            var updatedOrder = await _db.Orders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return Ok(updatedOrder);
        }

        // DELETE: api/orders/5 (Soft delete - promijeni status u Cancelled)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            // Provjeri da li postoji status "Cancelled"
            var cancelledStatus = await _db.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name.ToLower() == "cancelled");

            if (cancelledStatus != null)
            {
                order.OrderStatusId = cancelledStatus.OrderStatusId;
                await _db.SaveChangesAsync();
                return Ok(new { message = "Order cancelled successfully.", order });
            }

            return BadRequest("Cannot cancel order. Cancelled status not found in system.");
        }

        // Helper method za generisanje order numbera
        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }
    }

    // DTO classes
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int OrderStatusId { get; set; }
        public int ShippingMethodId { get; set; }
        public int PaymentMethodId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public int ProductOptionId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateStatusRequest
    {
        public int OrderStatusId { get; set; }
    }
}