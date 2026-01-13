using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Modique.Application.DTOs.Auth;
using Modique.Application.Interfaces;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Modique.Application.Services;

public class AuthService : IAuthService
{
    private readonly ModiqueDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ModiqueDbContext db,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            throw new InvalidOperationException("Email already exists.");
        }

        string roleName = request.Role?.ToLower() switch
        {
            "admin" => "Administrator",
            "user" => "Customer",
            _ => "Customer"
        };

        var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (userRole == null)
        {
            _logger.LogError("Role '{RoleName}' not found in database", roleName);
            throw new InvalidOperationException($"System configuration error: Role '{roleName}' not found.");
        }

        string passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, passwordSalt);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RoleId = userRole.RoleId,
            RegistrationDate = DateTime.UtcNow,
            Active = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _db.Entry(user).Reference(u => u.Role).LoadAsync();

        _logger.LogInformation("New user registered: {Email} with role: {Role}", user.Email, userRole.Name);

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(6);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRole.Name
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!user.Active)
        {
            _logger.LogWarning("Login attempt for inactive user: {Email}", request.Email);
            throw new UnauthorizedAccessException("Account is inactive.");
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            string? expectedRole = request.Role.ToLower() switch
            {
                "admin" => "Administrator",
                "user" => "Customer",
                _ => null
            };

            if (expectedRole != null && user.Role?.Name != expectedRole)
            {
                _logger.LogWarning("Login attempt with incorrect role for user: {Email}. Expected: {ExpectedRole}, Actual: {ActualRole}", 
                    request.Email, expectedRole, user.Role?.Name);
                throw new UnauthorizedAccessException("Invalid role for this account.");
            }
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(6);

        _logger.LogInformation("Successful login for user: {Email} with role: {Role}", user.Email, user.Role?.Name);

        return new AuthResponseDto
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
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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

    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role?.Name
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            _logger.LogWarning("Failed password change attempt for user: {UserId}", userId);
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        string passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, passwordSalt);
        user.PasswordSalt = passwordSalt;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Password changed for user: {Email}", user.Email);
        return true;
    }
}
