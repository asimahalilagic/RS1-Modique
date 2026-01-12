using System.ComponentModel.DataAnnotations;

namespace Modique.Application.DTOs.Auth;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    [RegularExpression(@"^(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one number")]
    public string NewPassword { get; set; } = string.Empty;
}
