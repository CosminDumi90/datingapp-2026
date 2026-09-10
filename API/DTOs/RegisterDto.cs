using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(4, ErrorMessage = "Password must be at least 4 characters long.")]
    public string Password { get; set; } = string.Empty;
}