using Api.Repository.Models;
using Api.Repository.ViewModels;
// using ApiCrud.Services.Attributes;
using Api.Services.Services.Books;
using Api.Services.Attributes;
using Api.Services.Utilities.JWT;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiCrud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{

    private readonly IBookService _bookService;
    private readonly ITokenUtilities _tokenService;

    public BookController(IBookService bookService, ITokenUtilities tokenService)
    {
        _bookService = bookService;
        _tokenService = tokenService;
    }



    [HttpGet("Books", Name = "GetBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [CustomAuthorization]
    public async Task<IActionResult> GetBooks()
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync();

            return Ok(new ResponseModel
            {
                Data = books,
                Result = true,
                Message = "Books loaded successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel
            {
                Data = new List<Book>(),
                Result = false,
                Message = ex.Message
            });
        }
    }




    [HttpPost("AddBook", Name = "AddBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> AddBook([FromBody] BookDetails newBook)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseModel { Data = new object(), Result = false, Message = "Invalid model" });

        if (newBook.Id > 0)
            return BadRequest(new ResponseModel { Data = new object(), Result = false, Message = "Book ID must be 0 for new books." });

        try
        {
            int bookId = await _bookService.AddNewBookAsync(newBook, _tokenService.GetJWTToken(Request) ?? "");

            return Ok(new ResponseModel
            {
                Data = new { Id = bookId },
                Result = true,
                Message = "Book added successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
    }




    [HttpDelete("DeleteBook", Name = "DeleteBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (id == 0)
        {
            return BadRequest(new ResponseModel { Result = false, Message = "Invalid book ID" });
        }

        try
        {
            bool isDeleted = await _bookService.DeleteBookAsync(id, _tokenService.GetJWTToken(Request) ?? "");
            return Ok(new ResponseModel
            {
                Result = isDeleted,
                Message = "Book deleted successfully.",
                Data = new { Id = id }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Result = false,
                Message = ex.Message,
                Data = new { Id = id }
            });
        }
    }


    [HttpGet("EditBookData", Name = "GetEditBookData")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> GetBookData(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = "Invalid book ID."
            });
        }

        try
        {
            BookDetails book = await _bookService.GetBookByIdAsync(id);

            return Ok(new ResponseModel
            {
                Data = book,
                Result = true,
                Message = "Book loaded successfully."
            });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ResponseModel
                {
                    Data = new object(),
                    Result = false,
                    Message = ex.Message
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Data = new object(),
                Result = false,
                Message = ex.Message
            });
        }
    }



    [HttpPut("UpdateBook", Name = "UpdateBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> UpdateBook([FromBody] BookDetails book)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseModel { Result = false, Message = "Invalid model" });

        if (book.Id == 0)
            return BadRequest(new ResponseModel { Result = false, Message = "Book ID is required for update." });

        try
        {
            bool isUpdated = await _bookService.UpdateBookAsync(book, _tokenService.GetJWTToken(Request) ?? "");

            return Ok(new ResponseModel
            {
                Result = isUpdated,
                Data = new { Id = book.Id },
                Message = "Book updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Result = false,
                Data = new object(),
                Message = ex.Message
            });
        }
    }




    [HttpPatch("UpdateAvailability", Name = "UpdateAvailability")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> UpdateAvailability(int id, [FromBody] bool isavailable)
    {
        if (id <= 0 || !ModelState.IsValid)
            return BadRequest(new ResponseModel { Result = false, Message = "Invalid book ID or payload" });

        try
        {
            bool updated = await _bookService.UpdateAvailabilityAsync(id, isavailable, _tokenService.GetJWTToken(Request) ?? "");
            return Ok(new ResponseModel
            {
                Result = updated,
                Message = "Book availability updated successfully.",
                Data = new { Id = id, IsAvailable = isavailable }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
            {
                Result = false,
                Message = ex.Message,
                Data = new { Id = id, IsAvailable = isavailable }
            });
        }
    }

}