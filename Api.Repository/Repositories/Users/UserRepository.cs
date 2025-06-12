using Api.Repository.Data;
using Api.Repository.Repositories;
using Api.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly LibraryDBContext _context;

    public UserRepository(LibraryDBContext context)
    {
        _context = context;
    }


    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetUserByMobileNumberAsync(string Mobilenumber)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Mobilenumber.ToLower() == Mobilenumber.ToLower());
    }

    public async Task<bool?> CreateUserAsync(User user)
    {
        if (user == null) return false;
        await _context.Users.AddAsync(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<User>?> GetPaginatedUserOrderByListAsync(int pageNumber, int pageSize, string searchTerm)
    {

        searchTerm = searchTerm.ToLower();

        return await _context.Users
            .Where(x => x.Name.ToLower().Contains(searchTerm) ||
                        x.Email.ToLower().Contains(searchTerm) ||
                        x.Id.ToString().ToLower().Contains(searchTerm))
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalRecordsForSearchAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();
        return await _context.Users.Where(x => x.Name.ToLower().Contains(searchTerm) ||
                        x.Email.ToLower().Contains(searchTerm) ||
                        x.Id.ToString().ToLower().Contains(searchTerm)).CountAsync();
    }

    public async Task<int> GetTotalRecordsAsync()
    {
        return await _context.Users.CountAsync();
    }
}