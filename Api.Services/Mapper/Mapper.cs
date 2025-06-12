using Api.Repository.ViewModels;
using Api.Repository.Models;
using AutoMapper;


namespace Api.Services.Mapper;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Book, BookDetails>();
        CreateMap<BookDetails, Book>();
        CreateMap<CreateUserVM, User>();
        CreateMap<User, CreateUserVM>();
    }
}