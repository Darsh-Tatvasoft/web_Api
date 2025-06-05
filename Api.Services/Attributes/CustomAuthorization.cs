using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Api.Services.Utilities.JWT;
using Api.Repository.Repositories.Users;
using Api.Repository.Models;

namespace Api.Services.Attributes;

public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var jwtService = context.HttpContext.RequestServices.GetService(typeof(ITokenUtilities)) as ITokenUtilities;
        var userRepo = context.HttpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;
        if (jwtService == null || userRepo == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var token = jwtService.GetJWTToken(context.HttpContext.Request);

        ClaimsPrincipal? principal = null;
        try
        {
            principal = jwtService.ValidateToken(token ?? "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        if (principal == null)
        {
            var refreshToken = jwtService.GetRefereshToken(context.HttpContext.Request);
            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            ClaimsPrincipal? principalRefresh = null;
            try
            {
                principalRefresh = jwtService.ValidateRefreshToken(refreshToken);
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (principalRefresh == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var emailInRefreshToken = jwtService.GetEmailFromJWT(refreshToken);
            if (string.IsNullOrEmpty(emailInRefreshToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            User? user = await userRepo.GetUserByEmailAsync(emailInRefreshToken);
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            token = jwtService.GenerateJwtToken(user);
            jwtService.SaveJWTToken(context.HttpContext.Response, token);
            refreshToken = jwtService.GenerateRefreshToken(user.Email);
            jwtService.SaveRefreshJWTToken(context.HttpContext.Response, refreshToken);
            try
            {
                principal = jwtService.ValidateToken(token ?? "");
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (principal == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

        }
        context.HttpContext.User = principal;
    }
}
