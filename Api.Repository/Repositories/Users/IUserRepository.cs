using Api.Repository.Models;

namespace Api.Repository.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByMobileNumberAsync(string Mobilenumber);
    Task<bool?> CreateUserAsync(User user);
    Task<List<User>?> GetPaginatedUserOrderByListAsync(int pageNumber, int PageSize, string searchTerm);
    Task<int> GetTotalRecordsForSearchAsync(string searchTerm);
    Task<int> GetTotalRecordsAsync();
}