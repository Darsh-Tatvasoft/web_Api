
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Http;
using Api.Services.Utilities.JWT;
using Api.Repository.Models;

namespace Api.Services.Utilities.JWT;

public class TokenUtilities : ITokenUtilities
{
    private readonly string? _key;
    private readonly string? _issuer;
    private readonly string? _audience;

    public TokenUtilities(IConfiguration configuration)
    {
        _key = configuration["JwtSettings:Key"];
        _issuer = configuration["JwtSettings:Issuer"];
        _audience = configuration["JwtSettings:Audience"];
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (_key == null || _issuer == null || _audience == null)
        {
            throw new InvalidOperationException("JWT configuration is not properly set in appsettings.");
        }
        var key = Encoding.UTF8.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("userId", user.Id.ToString() ),
                }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void SaveJWTToken(HttpResponse response, string token)
    {
        response.Cookies.Append("JwtToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        var tokenHandler = new JwtSecurityTokenHandler();
        if (_key == null || _issuer == null || _audience == null)
        {
            throw new InvalidOperationException("JWT configuration is not properly set in appsettings.");
        }
        var key = Encoding.UTF8.GetBytes(_key);
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public string GenerateRefreshToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (_key == null || _issuer == null || _audience == null)
        {
            throw new InvalidOperationException("JWT configuration is not properly set in appsettings.");
        }
        var key = Encoding.UTF8.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Email, email)
                }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void SaveRefreshJWTToken(HttpResponse response, string token)
    {
        response.Cookies.Append("RefreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        var tokenHandler = new JwtSecurityTokenHandler();
        if (_key == null || _issuer == null || _audience == null)
        {
            throw new InvalidOperationException("JWT configuration is not properly set in appsettings.");
        }
        var key = Encoding.UTF8.GetBytes(_key);
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string? GetJWTToken(HttpRequest request)
    {
        _ = request.Cookies.TryGetValue("JwtToken", out string? token);
        return token;
    }

    public string? GetRefereshToken(HttpRequest request)
    {
        _ = request.Cookies.TryGetValue("RefreshToken", out string? token);
        return token;
    }

    public string? GetEmailFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken != null)
            {
                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email");
                if (email != null)
                {
                    return email.Value;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}

