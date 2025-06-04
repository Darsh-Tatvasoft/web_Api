using Api.Repository.Data;
using Api.Repository.Repositories.Books;
using Api.Repository.Repositories.Users;
using Api.Services.Services.Authentication;
using Api.Services.Services.Books;
using Api.Services.Utilities.JWT;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<LibraryDBContext>(q => q.UseNpgsql(conn));

// Add services to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ITokenUtilities, TokenUtilities>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
