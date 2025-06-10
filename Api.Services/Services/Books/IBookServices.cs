using Api.Repository.Models;
using Api.Repository.ViewModels;

namespace Api.Services.Services.Books;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync();
    Task<int> AddNewBookAsync(BookDetails bookDetails, string token);
    Task<bool> UpdateBookAsync(BookDetails bookDetails, string token);
    Task<BookDetails> GetBookByIdAsync(int bookId);
    Task<bool> DeleteBookAsync(int bookId, string token);
    Task<bool> UpdateAvailabilityAsync(int bookId, bool isAvailable, string token);
}