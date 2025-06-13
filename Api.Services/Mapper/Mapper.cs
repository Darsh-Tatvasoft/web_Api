using Api.Repository.ViewModels;
using Api.Repository.Models;
using AutoMapper;
using Api.Repository.Enum;


namespace Api.Services.Mapper;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Book, BookDetails>();
        CreateMap<BookDetails, Book>();
        CreateMap<CreateUserVM, User>();
        CreateMap<User, CreateUserVM>();
        CreateMap<User, UserDetails>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => ((UserRole)src.Role).ToString()));
        CreateMap<UserDetails, User>();
    }
}