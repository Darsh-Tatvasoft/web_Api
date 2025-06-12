using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Repository.ViewModels;

public partial class CreateUserVM
{
    [Required]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
    public string Mobilenumber { get; set; } = null!;
    [Required]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = null!;
    [Required]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}