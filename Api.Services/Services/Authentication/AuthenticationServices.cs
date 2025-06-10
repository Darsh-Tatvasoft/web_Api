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

    public async Task<(User User, string Token, string RefreshToken)> AuthenticateUser(LoginDetails loginUser)
    {
        if (string.IsNullOrWhiteSpace(loginUser.Email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(loginUser.Password))
            throw new ArgumentException("Password is required.");

        var user = await _userRepository.GetUserByEmailAsync(loginUser.Email);
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid password.");

        string token = _tokenService.GenerateJwtToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken(user.Email);

        return (user, token, refreshToken);
    }
}