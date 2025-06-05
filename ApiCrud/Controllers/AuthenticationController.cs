using Api.Repository.Models;
using Api.Repository.ViewModels;
// using ApiCrud.Services.Attributes;
using Api.Services.Services.Authentication;
using Api.Services.Utilities.JWT;
using Microsoft.AspNetCore.Mvc;

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


    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDetails model)
    {
        (User? user, string? message, string? token) = await _authenticationService.AuthenticateUser(model);
        if (message != "" || user == null)
        {
            return BadRequest(new { Message = message });
        }
        _tokenService.SaveJWTToken(Response, token ?? "");
        _tokenService.SaveRefreshJWTToken(Response, _tokenService.GenerateRefreshToken(user.Email));
        return Ok(new { Message = "Logged in" });
    }

}