using Api.Repository.Models;
using Api.Repository.ViewModels;
// using ApiCrud.Services.Attributes;
using Api.Services.Services.Authentication;
using Api.Services.Utilities.JWT;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiCrud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{

    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenUtilities _tokenService;

    public AuthenticationController(IAuthenticationService authenticationService, ITokenUtilities tokenService)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
    }


    // [HttpPost("login", Name = "Login")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task<IActionResult> Login([FromBody] LoginDetails model)
    // {
    //     (User? user, string? message, string? token) = await _authenticationService.AuthenticateUser(model);
    //     if (message != "" || user == null)
    //     {
    //         return BadRequest(new { Message = message });
    //     }
    //     _tokenService.SaveJWTToken(Response, token ?? "");
    //     _tokenService.SaveRefreshJWTToken(Response, _tokenService.GenerateRefreshToken(user.Email));
    //     return Ok(new { Message = "Logged in" });
    // }

    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDetails model)
    {
        try
        {
            (User user, string token, string refreshToken) = await _authenticationService.AuthenticateUser(model);

            return Ok(new ResponseModel
            {
                Data = new
                {
                    token,
                    refreshToken
                },
                Result = true,
                Message = "Logged in"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error in {nameof(Login)}: {ex.Message}");
            // Log ex for internal diagnostics
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = "Something went wrong. Please try again later."
            });
        }
    }

    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateUserVM model)
    {
        try
        {
            (User user, string token, string refreshToken) = await _authenticationService.RegisterUser(model);

            return Ok(new ResponseModel
            {
                Data = new
                {
                    token,
                    refreshToken
                },
                Result = true,
                Message = "User registered successfully"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error in {nameof(Register)}: {ex.Message}");
            // Log ex for internal diagnostics
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = "Something went wrong. Please try again later."
            });
        }
    }


    [HttpPost("Tokens", Name = "Tokens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Tokens(string token)
    {
        try
        {
            ClaimsPrincipal? principal = _tokenService.ValidateRefreshToken(token);
            if (principal == null)
            {
                throw new UnauthorizedAccessException("invalide token!");
            }
            string? email = _tokenService.GetEmailFromJWT(token);
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email extracted from token is null or empty.");
            }
            (string tokens, string refreshToken) = await _authenticationService.GetNewTokens(email);
            ResponseModel responseModel = new()
            {
                Data = new
                {
                    token = tokens,
                    refreshToken = refreshToken
                },
                Result = true,
                Message = "Tokens generated successfully"
            };
            return Ok(responseModel);
        }
        catch (UnauthorizedAccessException ex)
        {
            ResponseModel responseModel = new ResponseModel
            {
                Message = ex.Message,
                Result = false
            };
            return Unauthorized(responseModel);
        }
        catch (ArgumentException ex)
        {
            ResponseModel responseModel = new ResponseModel
            {
                Message = ex.Message,
                Result = false
            };
            return BadRequest(responseModel);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error in {nameof(Tokens)}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = "Something went wrong. Please try again later."
            });
        }
    }

}