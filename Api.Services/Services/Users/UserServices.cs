using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Api.Repository.Repositories.Users;
using Api.Repository.ViewModels;
using Api.Repository.Models;
using AutoMapper;

namespace Api.Services.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
        }

        public async Task<UsersVM?> GetUserListData(string? searchTerm, int pageSize = 5, int pageNumber = 1)
        {
            searchTerm ??= "";

            int totalRecords = searchTerm != ""
                ? await _userRepository.GetTotalRecordsForSearchAsync(searchTerm)
                : await _userRepository.GetTotalRecordsAsync();

            if (totalRecords == 0)
                throw new Exception("No records found for the given search term.");

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, Math.Max(1, totalPages)));

            var users = await _userRepository.GetPaginatedUserOrderByListAsync(pageNumber, pageSize, searchTerm);
            if (users?.Count == 0)
                throw new Exception("No users found for the given search term.");

            var userDetails = _mapper.Map<List<UserDetails>>(users);

            return new UsersVM
            {
                Users = userDetails,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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