using Api.Repository.Models;
using Api.Repository.ViewModels;

namespace Api.Services.Services.Books;

public interface IBookService
{
    Task<(List<Book>? books, string? ErrorMessage)> GetAllBooksAsync();
    Task<(bool Success, string? ErrorMessage, int id)> AddNewBookAsync(BookDetails book, string token);
    Task<(bool Success, string? ErrorMessage)> UpdateBookAsync(BookDetails book, string token);
    Task<(BookDetails? book, string? ErrorMessage)> GetBookByIdAsync(int bookId);
    Task<(bool Success, string? ErrorMessage)> DeleteBookAsync(int bookId, string token);
}