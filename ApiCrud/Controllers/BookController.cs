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

            return Ok(new
            {
                data = books,
                result = true,
                message = "Books loaded successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                data = new List<Book>(),
                result = false,
                message = ex.Message
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
            return BadRequest(new { data = (object?)null, result = false, message = "Invalid model" });

        if (newBook.Id > 0)
            return BadRequest(new { data = (object?)null, result = false, message = "Book ID must be 0 for new books." });

        try
        {
            int bookId = await _bookService.AddNewBookAsync(newBook, _tokenService.GetJWTToken(Request) ?? "");

            return Ok(new
            {
                data = new { Id = bookId },
                result = true,
                message = "Book added successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                data = (object?)null,
                result = false,
                message = ex.Message
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
            return BadRequest(new { result = false, message = "Invalid book ID" });
        }

        try
        {
            bool isDeleted = await _bookService.DeleteBookAsync(id, _tokenService.GetJWTToken(Request) ?? "");
            return Ok(new
            {
                result = isDeleted,
                message = "Book deleted successfully.",
                data = new { Id = id }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                result = false,
                message = ex.Message,
                data = new { Id = id }
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
            return BadRequest(new
            {
                data = (object?)null,
                result = false,
                message = "Invalid book ID."
            });
        }

        try
        {
            BookDetails book = await _bookService.GetBookByIdAsync(id);

            return Ok(new
            {
                data = book,
                result = true,
                message = "Book loaded successfully."
            });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new
                {
                    data = (object?)null,
                    result = false,
                    message = ex.Message
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                data = (object?)null,
                result = false,
                message = ex.Message
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
            return BadRequest(new { result = false, message = "Invalid model" });

        if (book.Id == 0)
            return BadRequest(new { result = false, message = "Book ID is required for update." });

        try
        {
            bool isUpdated = await _bookService.UpdateBookAsync(book, _tokenService.GetJWTToken(Request) ?? "");

            return Ok(new
            {
                result = isUpdated,
                data = new { Id = book.Id },
                message = "Book updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                result = false,
                data = (object?)null,
                message = ex.Message
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
            return BadRequest(new { result = false, message = "Invalid book ID or payload" });

        try
        {
            bool updated = await _bookService.UpdateAvailabilityAsync(id, isavailable, _tokenService.GetJWTToken(Request) ?? "");
            return Ok(new
            {
                result = updated,
                message = "Book availability updated successfully.",
                data = new { Id = id, IsAvailable = isavailable }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                result = false,
                message = ex.Message,
                data = new { Id = id, IsAvailable = isavailable }
            });
        }
    }

}