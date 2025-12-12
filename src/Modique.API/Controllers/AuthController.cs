using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Domain.DTO.Auth;
using Modique.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Modique.DTO.Auth;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ModiqueDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ModiqueDbContext db,
            IConfiguration config,
            ILogger<AuthController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Provjeri da li email već postoji
                if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                {
                    _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                    return BadRequest(new { message = "Registration failed." });
                }

                // Pronađi default User role
                var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole == null)
                {
                    _logger.LogError("Default 'User' role not found in database");
                    return StatusCode(500, new { message = "System configuration error." });
                }

                // Kreiraj novog korisnika
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email.ToLower(), // Normalizuj email
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RoleId = userRole.RoleId,
                    RegistrationDate = DateTime.UtcNow,
                    Active = true
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Email}", user.Email);

                // Vrati minimalne info
                return Ok(new
                {
                    user.UserId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    message = "Registration successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Pronađi korisnika sa role-om
                var user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                // Provjeri da li je user aktivan
                if (!user.Active)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Email}", request.Email);
                    return Unauthorized(new { message = "Account is inactive." });
                }

                // Verifikuj password
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                // Generiši JWT token
                var token = GenerateJwtToken(user);
                var expiresAt = DateTime.UtcNow.AddHours(6);

                _logger.LogInformation("Successful login for user: {Email}", user.Email);

                return Ok(new AuthResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role?.Name
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            // Implementacija za refresh token ako trebaš
            return Ok(new { message = "Refresh token endpoint - to be implemented" });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Jedinstveni ID tokena
            };

            // Dodaj role claim ako postoji
            if (!string.IsNullOrEmpty(user.Role?.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"] ?? _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}