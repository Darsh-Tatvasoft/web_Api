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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [CustomAuthorization]
    public async Task<IActionResult> GetBooks()
    {
        (List<Book>? books, string? errorMessage) = await _bookService.GetAllBooksAsync();
        if (books?.Count == 0)
        {
            return BadRequest();
        }
        return Ok(books);
    }



    [HttpPost("AddBook", Name = "AddBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [CustomAuthorization]
    public async Task<IActionResult> AddBook([FromBody] BookDetails newBook)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (newBook.Id > 0)
        {
            return BadRequest();
        }
        bool isAdded;
        int id;
        string? message;
        (isAdded, message, id) = await _bookService.AddNewBookAsync(newBook, _tokenService.GetJWTToken(Request) ?? "");
        if (!isAdded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return Ok(new { Id = id, Message = message });
    }



    [HttpDelete("DeleteBook", Name = "DeleteBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [CustomAuthorization]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        bool isDeleted;
        string? message;
        (isDeleted, message) = await _bookService.DeleteBookAsync(id, _tokenService.GetJWTToken(Request) ?? "");
        if (!isDeleted)
        {
            return BadRequest(new { Id = id, Message = message });
        }
        return Ok(new { Id = id, Message = message });
    }


    [HttpGet("EditBookData", Name = "GetEditBookData")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [CustomAuthorization]
    public async Task<IActionResult> GetBookData(int id)
    {
        (BookDetails? book, string? errorMessage) = await _bookService.GetBookByIdAsync(id);
        if (errorMessage != "")
        {
            return BadRequest(new { Message = errorMessage });
        }
        if (book == null)
        {
            return NotFound(new { Message = "Book not found." });
        }
        return Ok(book);
    }

    [HttpPut("UpdateBook", Name = "UpdateBook")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [CustomAuthorization]
    public async Task<IActionResult> UpdateBook([FromBody] BookDetails book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (book.Id == 0)
        {
            return BadRequest();
        }
        bool isUpdate;
        string? message;
        (isUpdate, message) = await _bookService.UpdateBookAsync(book, _tokenService.GetJWTToken(Request) ?? "");
        if (!isUpdate)
        {
            return BadRequest(new { Id = book.Id, Message = message });
        }
        return Ok(new { Id = book.Id, Message = message });
    }



    [HttpPatch("UpdateAvailability", Name = "UpdateAvailability")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [CustomAuthorization]
    public async Task<IActionResult> UpdateAvailability(int id, [FromBody] bool isavailable)
    {
        if (id <= 0 || !ModelState.IsValid)
            return BadRequest("Invalid book ID or payload");

        var updated = await _bookService.UpdateAvailabilityAsync(id, isavailable, _tokenService.GetJWTToken(Request) ?? "");
        if (!updated.Success)
        {
            return BadRequest(new { Message = updated.ErrorMessage });
        }
        if (updated.ErrorMessage != "")
        {
            return BadRequest(new { Message = updated.ErrorMessage });
        }
        return Ok();
    }

}