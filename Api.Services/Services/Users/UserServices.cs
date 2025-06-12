using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Api.Repository.Repositories.Users;
using Api.Repository.ViewModels;
using Api.Repository.Models;

namespace Api.Services.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<UsersVM?> GetUserListData(string? searchTerm, int pageSize = 5, int pageNumber = 1)
        {
            searchTerm = searchTerm ?? "";
            int totalRecords = await _userRepository.GetTotalRecordsAsync();
            if (searchTerm != "")
            {
                totalRecords = await _userRepository.GetTotalRecordsForSearchAsync(searchTerm);
            }

            if (totalRecords == 0)
            {
                throw new Exception("No records found for the given search term.");
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, Math.Max(1, totalPages)));
            List<User>? users = await _userRepository.GetPaginatedUserOrderByListAsync(pageNumber, pageSize, searchTerm);
            if (users?.Count() == 0)
            {
                throw new Exception("No users found for the given search term.");
            }
            var paginationData = new UsersVM
            {
                Users = users,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return paginationData;

        }

        // public async Task<(User? user, string? ErrorMessage)> GetUserDetails(int Id)
        // {
        //     try
        //     {
        //         User? user = await _userRepository.GetUserByIdAsync(Id);

        //         if (user == null)
        //         {
        //             return (null, "User Not Found.");
        //         }

        //         return (user, null);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"[GetUserDetails] Error: {ex.Message}");
        //         return (null, "An error occurred while fetching user details. Please try again later.");
        //     }
        // }
    }
}