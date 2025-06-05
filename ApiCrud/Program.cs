using Api.Repository.Data;
using Api.Repository.Repositories.Books;
using Api.Repository.Repositories.Users;
using Api.Services.Services.Authentication;
using Api.Services.Services.Books;
using Api.Services.Utilities.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Database ------------------
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<LibraryDBContext>(q => q.UseNpgsql(conn));

// ------------------ CORS ------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
    });
});

// ------------------ Repositories & Services ------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ITokenUtilities, TokenUtilities>();

// ------------------ AutoMapper & Controllers ------------------
builder.Services.AddAutoMapper(typeof(Api.Services.Mapper.Mapper));
builder.Services.AddControllers();



// ------------------ Swagger ------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"Bearer token."
    });

    c.AddSecurityDefinition("RefreshToken", new OpenApiSecurityScheme
    {
        Name = "Refresh-Token",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "RefreshToken",
        In = ParameterLocation.Header,
        Description = "Refresh token header"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "RefreshToken" }
            },
            new List<string>()
        }
    });
});

// ------------------ App Pipeline ------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthorization(); // Authentication middleware removed

app.MapControllers();

app.Run();