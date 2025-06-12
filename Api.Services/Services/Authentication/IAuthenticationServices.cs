using Api.Repository.Models;
using Api.Repository.ViewModels;

namespace Api.Services.Services.Authentication;

public interface IAuthenticationService
{
    Task<(User User, string Token, string RefreshToken)> AuthenticateUser(LoginDetails loginUser);

    Task<(User User, string Token, string RefreshToken)> RegisterUser(CreateUserVM createUser);
}