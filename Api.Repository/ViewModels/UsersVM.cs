using Api.Repository.Models;
namespace Api.Repository.ViewModels;

public class UsersVM
{
    public List<User>? Users { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
