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
}