using Api.Repository.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Services.Utilities.JWT;

public interface ITokenUtilities
{
    public string GenerateJwtToken(User user);
    public void SaveJWTToken(HttpResponse response, string token);
    public ClaimsPrincipal? ValidateToken(string token);
    public string GenerateRefreshToken(string email);
    public void SaveRefreshJWTToken(HttpResponse response, string token);
    public ClaimsPrincipal? ValidateRefreshToken(string token);


    public string? GetJWTToken(HttpRequest request);
    public string? GetRefereshToken(HttpRequest request);
    public string? GetEmailFromJWT(string token);
}