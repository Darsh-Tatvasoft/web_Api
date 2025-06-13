using Api.Repository.Models;
namespace Api.Repository.ViewModels;

public class UserDetails
{
    public int Id { get; set; }
    public string Role { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Mobilenumber { get; set; } = null!;
    public string Email { get; set; } = null!;
}

