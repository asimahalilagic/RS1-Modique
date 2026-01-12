using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modique.Application.DTOs.Auth;
using Modique.Application.Interfaces;
using System.Security.Claims;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        /// <summary>
        /// Login user and get JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }

        /// <summary>
        /// Get current logged in user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _authService.GetCurrentUserAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred." });
            }
        }

        /// <summary>
        /// Change password for logged in user
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                await _authService.ChangePasswordAsync(userId, request);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "An error occurred while changing password." });
            }
        }

        /// <summary>
        /// Refresh JWT token (to be implemented)
        /// </summary>
        [HttpPost("refresh")]
        [Authorize]
        public IActionResult RefreshToken()
        {
            return Ok(new { message = "Refresh token endpoint - to be implemented" });
        }
    }
}
