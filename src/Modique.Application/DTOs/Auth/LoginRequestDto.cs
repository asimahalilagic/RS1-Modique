using System.ComponentModel.DataAnnotations;

namespace Modique.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [RegularExpression(@"^(User|Admin)?$", ErrorMessage = "Role must be either 'User' or 'Admin' if provided")]
    public string? Role { get; set; }
}




