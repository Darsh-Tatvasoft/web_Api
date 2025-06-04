using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Repository.ViewModels;

public partial class LoginDetails
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
    public int Id { get; set; }
}