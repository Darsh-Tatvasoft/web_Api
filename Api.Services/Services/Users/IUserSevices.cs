using Api.Repository.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Api.Services.Services.Users
{
    public interface IUserService
    {
        Task<UsersVM?> GetUserListData(string? searchTerm, int pageSize = 5, int pageNumber = 1);
        // Task<(User? user, string? ErrorMessage)> GetUserDetails(int Id);
    }
}