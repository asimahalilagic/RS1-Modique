using Modique.Application.DTOs.Auth;

namespace Modique.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserDto> GetCurrentUserAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request);
}

