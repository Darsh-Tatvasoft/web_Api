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

    public async Task<(List<Book>? books, string? ErrorMessage)> GetAllBooksAsync()
    {
        try
        {
            var books = await _bookRepository.GetAllBooksAsync();
            if (books == null || !books.Any())
            {
                return (null, "No books found.");
            }
            return (books, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetAllBooksAsync] Error: {ex.Message}");
            return (null, "Failed to retrieve books.");
        }
    }

    public async Task<(bool Success, string? ErrorMessage, int id)> AddNewBookAsync(BookDetails bookDetails, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return (false, "Token is required.", 0);

            var user = _tokenService.ValidateToken(token);
            // if (user == null || !user.IsAdmin)
            //     return (false, "Unauthorized access.");
            string? email = _tokenService.GetEmailFromJWT(token);
            User? existingUser = new User();
            if (email != null)
            {
                existingUser = await _userRepository.GetUserByEmailAsync(email);
                if (existingUser == null)
                    return (false, "Unauthorized access.", 0);
            }
            // var book = new Book
            // {
            //     Title = bookDetails.Title,
            //     Author = bookDetails.Author,
            //     Isbn = bookDetails.Isbn,
            //     Publisheddate = bookDetails.Publisheddate,
            //     Language = bookDetails.Language,
            //     Publisher = bookDetails.Publisher,
            //     Price = bookDetails.Price,
            //     Pagecount = bookDetails.Pagecount,
            //     Stockquantity = bookDetails.Stockquantity,
            //     Isavailable = bookDetails.Isavailable ?? true,
            //     Genre = bookDetails.Genre,
            //     Createdat = DateTime.UtcNow,
            //     Createdby = existingUser.Id
            // };
            Book book = _mapper.Map<Book>(bookDetails);

            (bool? result, int id) = await _bookRepository.AddNewBookData(book);
            return result.HasValue && result == true ? (true, "", id) : (false, "Failed to add new book.", 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddNewBookAsync] Error: {ex.Message}");
            return (false, "Failed to add new book.", 0);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateBookAsync(BookDetails bookDetails, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return (false, "Token is required.");

            var user = _tokenService.ValidateToken(token);
            if (user == null)
                return (false, "Unauthorized access.");
            string? email = _tokenService.GetEmailFromJWT(token);
            User? existingUser = new User();
            if (email != null)
            {
                existingUser = await _userRepository.GetUserByEmailAsync(email);
                if (existingUser == null)
                    return (false, "Unauthorized access.");
            }
            Book? book = await _bookRepository.GetBookByIdAsync(bookDetails.Id);

            if (book == null)
                return (false, "Book not found.");
            book.Title = bookDetails.Title;
            book.Author = bookDetails.Author;
            book.Isbn = bookDetails.Isbn;
            book.Publisheddate = bookDetails.Publisheddate;
            book.Language = bookDetails.Language;
            book.Publisher = bookDetails.Publisher;
            book.Price = bookDetails.Price;
            book.Pagecount = bookDetails.Pagecount;
            book.Stockquantity = bookDetails.Stockquantity;
            book.Isavailable = bookDetails.Isavailable ?? true;
            book.Genre = bookDetails.Genre;
            book.Updatedat = DateTime.UtcNow;
            book.Updatedby = existingUser.Id;

            bool? result = await _bookRepository.UpdateBookData(book);
            return result.HasValue && result == true ? (true, "") : (false, "Failed to update book.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateBookAsync] Error: {ex.Message}");
            return (false, "Failed to update book.");
        }
    }

    public async Task<(BookDetails? book, string? ErrorMessage)> GetBookByIdAsync(int bookId)
    {
        try
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return (null, "Book not found.");
            }
            // var bookDetails = new BookDetails
            // {
            //     Id = book.Id,
            //     Title = book.Title,
            //     Author = book.Author,
            //     Isbn = book.Isbn,
            //     Publisheddate = book.Publisheddate,
            //     Language = book.Language,
            //     Publisher = book.Publisher,
            //     Price = book.Price,
            //     Pagecount = book.Pagecount,
            //     Stockquantity = book.Stockquantity,
            //     Isavailable = book.Isavailable,
            //     Genre = book.Genre
            // };
            BookDetails bookDetails = _mapper.Map<BookDetails>(book);

            return (bookDetails, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetBookByIdAsync] Error: {ex.Message}");
            return (null, "Failed to retrieve book.");
        }
    }


    public async Task<(bool Success, string? ErrorMessage)> DeleteBookAsync(int bookId, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return (false, "Token is required.");

            var user = _tokenService.ValidateToken(token);
            // if (user == null || !user.IsAdmin)
            //     return (false, "Unauthorized access.");
            string? email = _tokenService.GetEmailFromJWT(token);
            User? existingUser = new User();
            if (email != null)
            {
                existingUser = await _userRepository.GetUserByEmailAsync(email);
                if (existingUser == null)
                    return (false, "Unauthorized access.");
            }
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null)
                return (false, "Book not found.");

            book.Isdeleted = true;
            book.Updatedat = DateTime.UtcNow;
            book.Updatedby = existingUser.Id;
            bool? result = await _bookRepository.UpdateBookData(book);
            return result.HasValue && result == true ? (true, "") : (false, "Failed to delete book.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DeleteBookAsync] Error: {ex.Message}");
            return (false, "Failed to delete book.");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateAvailabilityAsync(int bookId, bool isAvailable, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return (false, "Token is required.");

            var user = _tokenService.ValidateToken(token);
            string? email = _tokenService.GetEmailFromJWT(token);
            User? existingUser = new User();
            if (email != null)
            {
                existingUser = await _userRepository.GetUserByEmailAsync(email);
                if (existingUser == null)
                    return (false, "Unauthorized access.");
            }
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null)
                return (false, "Book not found.");

            book.Isavailable = isAvailable;
            book.Updatedat = DateTime.UtcNow;
            book.Updatedby = existingUser.Id;
            bool? result = await _bookRepository.UpdateBookData(book);
            return result.HasValue && result == true ? (true, "") : (false, "Failed to update book availability.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateAvailabilityAsync] Error: {ex.Message}");
            return (false, "Failed to update book availability.");
        }
    }


}