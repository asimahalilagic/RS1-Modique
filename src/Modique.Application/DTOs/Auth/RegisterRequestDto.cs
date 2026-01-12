using System.ComponentModel.DataAnnotations;

namespace Modique.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    [RegularExpression(@"^(?=.*\d).+$", 
        ErrorMessage = "Password must contain at least one number")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'")]
    public string Role { get; set; } = "User";
}




