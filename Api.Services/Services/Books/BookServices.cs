// PizzaShop.Logic/Services/Authentication/AuthenticationService.cs
using Api.Repository.ViewModels;
using Api.Repository.Models;
using Api.Repository.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Api.Services.Utilities.JWT;
using Api.Repository.Repositories.Books;
using AutoMapper;

namespace Api.Services.Services.Books;

public class BookService : IBookService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenUtilities _tokenService;
    private readonly IBookRepository _bookRepository;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public BookService(IUserRepository userRepository, IBookRepository bookRepository, ITokenUtilities tokenService, IConfiguration config, IMapper mapper)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _config = config;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllBooksAsync();
        if (books == null || !books.Any())
        {
            throw new Exception("No books found.");
        }

        return books;
    }


    public async Task<int> AddNewBookAsync(BookDetails bookDetails, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Token is required.");

        var user = _tokenService.ValidateToken(token);
        string? email = _tokenService.GetEmailFromJWT(token);

        if (string.IsNullOrEmpty(email))
            throw new Exception("Unauthorized access.");

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            Book book = _mapper.Map<Book>(bookDetails);
            book.Createdby = existingUser.Id;
            book.Createdat = DateTime.UtcNow;

            var (result, id) = await _bookRepository.AddNewBookData(book);

            if (result != true)
                throw new Exception("Failed to add new book.");

            return id;
        }

        throw new Exception("Unauthorized access.");
    }


    public async Task<bool> UpdateBookAsync(BookDetails bookDetails, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Token is required.");

        var user = _tokenService.ValidateToken(token);
        if (user == null)
            throw new Exception("Unauthorized access.");

        string? email = _tokenService.GetEmailFromJWT(token);
        if (string.IsNullOrEmpty(email))
            throw new Exception("Unauthorized access.");

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser == null)
            throw new Exception("Unauthorized access.");

        Book? book = await _bookRepository.GetBookByIdAsync(bookDetails.Id);
        if (book == null)
            throw new Exception("Book not found.");

        _mapper.Map(bookDetails, book);
        book.Updatedat = DateTime.UtcNow;
        book.Updatedby = existingUser.Id;

        bool? result = await _bookRepository.UpdateBookData(book);
        if (result != true)
            throw new Exception("Failed to update book.");

        return true;
    }


    public async Task<BookDetails> GetBookByIdAsync(int bookId)
    {
        var book = await _bookRepository.GetBookByIdAsync(bookId);
        if (book == null)
            throw new Exception("Book not found.");

        return _mapper.Map<BookDetails>(book);
    }



    public async Task<bool> DeleteBookAsync(int bookId, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Token is required.");

        var user = _tokenService.ValidateToken(token);
        string? email = _tokenService.GetEmailFromJWT(token);

        if (string.IsNullOrEmpty(email))
            throw new Exception("Unauthorized access.");

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser == null)
            throw new Exception("Unauthorized access.");

        var book = await _bookRepository.GetBookByIdAsync(bookId);
        if (book == null)
            throw new Exception("Book not found.");

        book.Isdeleted = true;
        book.Updatedat = DateTime.UtcNow;
        book.Updatedby = existingUser.Id;

        bool? result = await _bookRepository.UpdateBookData(book);
        if (result != true)
            throw new Exception("Failed to delete book.");

        return true;
    }


    public async Task<bool> UpdateAvailabilityAsync(int bookId, bool isAvailable, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Token is required.");

        var user = _tokenService.ValidateToken(token);
        string? email = _tokenService.GetEmailFromJWT(token);

        if (string.IsNullOrEmpty(email))
            throw new Exception("Unauthorized access.");

        var existingUser = await _userRepository.GetUserByEmailAsync(email) ?? throw new Exception("Unauthorized access.");
        var book = await _bookRepository.GetBookByIdAsync(bookId);
        if (book == null)
            throw new Exception("Book not found.");

        book.Isavailable = isAvailable;
        book.Updatedat = DateTime.UtcNow;
        book.Updatedby = existingUser.Id;

        bool? result = await _bookRepository.UpdateBookData(book);
        if (result != true)
            throw new Exception("Failed to update book availability.");

        return true;
    }



}