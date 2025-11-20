using System.ComponentModel.DataAnnotations;

namespace EmployeesManagementSystem.DTOs;

#nullable enable

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? Password { get; set; }  // Optional - only set if changing password
}