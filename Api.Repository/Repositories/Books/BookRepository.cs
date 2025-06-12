using Api.Repository.Data;
using Api.Repository.Repositories;
using Api.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository.Repositories.Books;

public class BookRepository : IBookRepository
{
    private readonly LibraryDBContext _context;

    public BookRepository(LibraryDBContext context)
    {
        _context = context;
    }

    public async Task<List<Book>?> GetAllBooksAsync()
    {
        try
        {
            return await _context.Books.Where(x => x.Isdeleted == false).OrderBy(x => x.Id).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetAllBooksAsync] Error: {ex.Message}");
            return null;
        }
    }

    public async Task<(bool? success, int id)> AddNewBookData(Book book)
    {
        try
        {
            if (book == null)
                return (false, 0);

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return (true, book.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddNewBookData] Error: {ex.Message}");
            return (false, 0);
        }
    }

    public async Task<bool?> UpdateBookData(Book book)
    {
        try
        {
            if (book == null)
                return false;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateBookData] Error: {ex.Message}");
            return false;
        }
    }

    public async Task<Book?> GetBookByIdAsync(int bookId)
    {
        try
        {
            return await _context.Books.FindAsync(bookId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetBookByIdAsync] Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Book?> GetBookByIsbnAsync(string Isbn)
    {
        try
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Isbn == Isbn && b.Isdeleted == false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetBookByIsbnAsync] Error: {ex.Message}");
            return null;
        }
    }
}