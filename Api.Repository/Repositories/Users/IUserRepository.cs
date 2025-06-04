using Api.Repository.Models;

namespace Api.Repository.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);

}