using System.ComponentModel.DataAnnotations;

namespace EventEase_App.Models;

public sealed class RegistrationModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 80 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;
}
