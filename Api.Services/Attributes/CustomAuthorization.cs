// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using System.Security.Claims;
// using System.IdentityModel.Tokens.Jwt;
// using Api.Services.Utilities.JWT;
// using Api.Repository.Repositories.Users;
// using Api.Repository.Models;

// namespace Api.Services.Attributes;




// public class CustomAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
// {
//     public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//     {
//         var jwtService = context.HttpContext.RequestServices.GetService(typeof(ITokenUtilities)) as ITokenUtilities;
//         var userRepo = context.HttpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;

//         if (jwtService == null || userRepo == null)
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }

//         string? token = jwtService.GetJWTToken(context.HttpContext.Request);
//         ClaimsPrincipal? principal = null;

//         if (!string.IsNullOrEmpty(token))
//         {
//             try
//             {
//                 principal = jwtService.ValidateToken(token);
//             }
//             catch
//             {
//                 // Token invalid, will try refresh token
//             }
//         }

//         if (principal == null)
//         {
//             // Try Refresh Token
//             string? refreshToken = jwtService.GetRefereshToken(context.HttpContext.Request);
//             if (string.IsNullOrEmpty(refreshToken))
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             ClaimsPrincipal? refreshPrincipal = null;
//             try
//             {
//                 refreshPrincipal = jwtService.ValidateRefreshToken(refreshToken);
//             }
//             catch
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             if (refreshPrincipal == null)
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             string? emailInRefreshToken = jwtService.GetEmailFromJWT(refreshToken);
//             if (string.IsNullOrEmpty(emailInRefreshToken))
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             User? user = await userRepo.GetUserByEmailAsync(emailInRefreshToken);
//             if (user == null)
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             // Generate new tokens and save in response cookies
//             token = jwtService.GenerateJwtToken(user);
//             jwtService.SaveJWTToken(context.HttpContext.Response, token);

//             refreshToken = jwtService.GenerateRefreshToken(user.Email);
//             jwtService.SaveRefreshJWTToken(context.HttpContext.Response, refreshToken);

//             try
//             {
//                 principal = jwtService.ValidateToken(token);
//             }
//             catch
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             if (principal == null)
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }
//         }

//         // Set the authenticated user
//         context.HttpContext.User = principal;
//     }
// }


// public class CustomAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
// {
//     public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//     {
//         var jwtService = context.HttpContext.RequestServices.GetService(typeof(ITokenUtilities)) as ITokenUtilities;
//         var userRepo = context.HttpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;

//         if (jwtService == null || userRepo == null)
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }

//         var req = context.HttpContext.Request;
//         var res = context.HttpContext.Response;

//         string? token = req.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
//         string? refreshToken = req.Headers["Refresh-Token"].FirstOrDefault();

//         ClaimsPrincipal? principal = null;

//         if (!string.IsNullOrEmpty(token))
//         {
//             try { principal = jwtService.ValidateToken(token); }
//             catch { /* Invalid token, fallback to refresh */ }
//         }

//         if (principal == null && !string.IsNullOrEmpty(refreshToken))
//         {
//             try
//             {
//                 var refreshPrincipal = jwtService.ValidateRefreshToken(refreshToken);
//                 var email = jwtService.GetEmailFromJWT(refreshToken);
//                 if (string.IsNullOrEmpty(email))
//                 {
//                     context.Result = new UnauthorizedResult();
//                     return;
//                 }

//                 var user = await userRepo.GetUserByEmailAsync(email);
//                 if (user == null)
//                 {
//                     context.Result = new UnauthorizedResult();
//                     return;
//                 }

//                 token = jwtService.GenerateJwtToken(user);
//                 refreshToken = jwtService.GenerateRefreshToken(user.Email);

//                 res.Headers["Authorization"] = $"Bearer {token}";
//                 res.Headers["Refresh-Token"] = refreshToken;

//                 principal = jwtService.ValidateToken(token);
//             }
//             catch
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }
//         }

//         if (principal == null)
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }

//         context.HttpContext.User = principal;
//     }
// }







using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Api.Services.Utilities.JWT;
using Api.Repository.Repositories.Users;
using Api.Repository.Models;
using System.Threading.Tasks;

namespace Api.Services.Attributes;

public class CustomAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get required services from DI
        var jwtService = context.HttpContext.RequestServices.GetService(typeof(ITokenUtilities)) as ITokenUtilities;

        if (jwtService == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Extract token from headers
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        ClaimsPrincipal? principal = null;

        // Validate the token
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                principal = jwtService.ValidateToken(token);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            }
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // If principal is still null, return unauthorized
        if (principal == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Set the principal and add token to response headers
        context.HttpContext.User = principal;
        context.HttpContext.Response.Headers["Authorization"] = $"Bearer {token}";
    }
}


