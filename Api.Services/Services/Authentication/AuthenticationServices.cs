// PizzaShop.Logic/Services/Authentication/AuthenticationService.cs
using Api.Repository.ViewModels;
using Api.Repository.Models;
using Api.Repository.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Api.Services.Utilities.JWT;

namespace Api.Services.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenUtilities _tokenService;
    private readonly IConfiguration _config;

    public AuthenticationService(IUserRepository userRepository, ITokenUtilities tokenService, IConfiguration config)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<(User? User, string? ErrorMessage, string? token)> AuthenticateUser(LoginDetails loginUser)
    {
        try
        {
            if (string.IsNullOrEmpty(loginUser.Email))
                return (null, "Email is required.", "");
            if (string.IsNullOrEmpty(loginUser.Password))
                return (null, "Password is required.", "");

            var user = await _userRepository.GetUserByEmailAsync(loginUser.Email);
            if (user == null)
                return (null, "User not found.", "");


            // await _userRepository.UpdateUserAsync(user);

            if (!BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
                return (null, "Invalid password.", "");

            return (user, "", _tokenService.GenerateJwtToken(user));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthenticateUser] Error: {ex.Message}");
            return (null, "Something went wrong. Please try again later.", "");
        }
    }
}