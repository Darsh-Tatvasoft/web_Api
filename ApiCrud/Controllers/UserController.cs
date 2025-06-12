using Api.Services.Services.Books;
using Api.Services.Utilities.JWT;
using Microsoft.AspNetCore.Mvc;
using Api.Services.Services.Users;
using Api.Services.Attributes;
using Api.Repository.ViewModels;
using Api.Repository.Models;

namespace ApiCrud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet("Users", Name = "GetUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [CustomAuthorization]
    public async Task<IActionResult> GetUserData(string? searchTerm, int pageSize = 5, int pageNumber = 1)
    {
        try
        {
            UsersVM? users = await _userService.GetUserListData(searchTerm, pageSize, pageNumber);
            if (users == null)
            {
                return NotFound(new { Message = "No users found." });
            }

            return Ok(new ResponseModel
            {
                Data = users,
                Result = true,
                Message = "Users loaded successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel
            {
                Data = new List<User>(),
                Result = false,
                Message = ex.Message
            });
        }
    }
}