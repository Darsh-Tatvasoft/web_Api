using Api.Repository.Models;

namespace Api.Repository.Repositories.Books;

public interface IBookRepository
{
    Task<List<Book>?> GetAllBooksAsync();
    Task<(bool? success, int id)> AddNewBookData(Book book);
    Task<bool?> UpdateBookData(Book book);
    Task<Book?> GetBookByIdAsync(int bookId);
}