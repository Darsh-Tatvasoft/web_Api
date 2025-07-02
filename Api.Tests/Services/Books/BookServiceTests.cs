using Xunit;
using Moq;
using Api.Repository.Repositories.Users;
using Api.Repository.Repositories.Books;
using Api.Services.Utilities.JWT;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Api.Services.Services.Books;
using Api.Repository.Models;
using Api.Repository.ViewModels;

namespace Api.Tests.Services.Books;

public class BookServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<ITokenUtilities> _mockTokenUtils;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfig;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockBookRepo = new Mock<IBookRepository>();
        _mockTokenUtils = new Mock<ITokenUtilities>();
        _mockMapper = new Mock<IMapper>();
        _mockConfig = new Mock<IConfiguration>();

        _bookService = new BookService(
            _mockUserRepo.Object,
            _mockBookRepo.Object,
            _mockTokenUtils.Object,
            _mockConfig.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsListOfBooks_WhenBooksExist()
    {
        // Arrange
        var books = new List<Book> { new Book { Id = 1, Title = "Test Book", Author = "Author" } };
        _mockBookRepo.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(books);

        // Act
        var result = await _bookService.GetAllBooksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Book", result[0].Title);
    }

    [Fact]
    public async Task GetAllBooksAsync_ThrowsException_WhenNoBooksFound()
    {
        // Arrange
        _mockBookRepo.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(new List<Book>());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookService.GetAllBooksAsync());
    }

    [Fact]
    public async Task AddNewBookAsync_AddsBookSuccessfully_WhenValidDataProvided()
    {
        // Arrange
        var token = "valid.jwt.token";
        var email = "user@example.com";
        var user = new User { Id = 1, Email = email };
        var bookDetails = new BookDetails
        {
            Title = "Test Book",
            Author = "Author",
            Isbn = "1234567890",
            Genre = "Fiction",
            Language = "English",
            Publisher = "Publisher",
            Price = 10.5m,
            Pagecount = 200,
            Stockquantity = 10,
            Isavailable = true,
            Publisheddate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email)
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);

        _mockTokenUtils.Setup(t => t.ValidateToken(token)).Returns(claimsPrincipal);
        _mockTokenUtils.Setup(t => t.GetEmailFromJWT(token)).Returns(email);
        _mockUserRepo.Setup(u => u.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mockBookRepo.Setup(b => b.GetBookByIsbnAsync(bookDetails.Isbn)).ReturnsAsync((Book?)null);
        _mockMapper.Setup(m => m.Map<Book>(bookDetails)).Returns(new Book { Title = bookDetails.Title });
        _mockBookRepo.Setup(b => b.AddNewBookData(It.IsAny<Book>())).ReturnsAsync((true, 1));

        // Act
        var bookId = await _bookService.AddNewBookAsync(bookDetails, token);

        // Assert
        Assert.Equal(1, bookId);
        _mockBookRepo.Verify(b => b.AddNewBookData(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task AddNewBookAsync_ThrowsException_WhenTokenIsMissing()
    {
        // Arrange
        var bookDetails = new BookDetails
        {
            Title = "Test Book",
            Author = "Author",
            Isbn = "1234567890",
            Genre = "Fiction",
            Language = "English",
            Publisher = "Publisher",
            Price = 10.5m,
            Pagecount = 200,
            Stockquantity = 10,
            Isavailable = true,
            Publisheddate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _bookService.AddNewBookAsync(bookDetails, ""));
        Assert.Equal("Token is required.", ex.Message);
    }

    [Fact]
    public async Task AddNewBookAsync_ThrowsException_WhenBookWithIsbnAlreadyExists()
    {
        // Arrange
        var token = "valid.jwt.token";
        var email = "user@example.com";
        var user = new User { Id = 1, Email = email };
        var bookDetails = new BookDetails
        {
            Title = "Test Book",
            Author = "Author",
            Isbn = "1234567890",
            Genre = "Fiction",
            Language = "English",
            Publisher = "Publisher",
            Price = 10.5m,
            Pagecount = 200,
            Stockquantity = 10,
            Isavailable = true,
            Publisheddate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email)
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
        _mockTokenUtils.Setup(t => t.ValidateToken(token)).Returns(claimsPrincipal);
        _mockTokenUtils.Setup(t => t.GetEmailFromJWT(token)).Returns(email);
        _mockUserRepo.Setup(u => u.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mockBookRepo.Setup(b => b.GetBookByIsbnAsync(bookDetails.Isbn)).ReturnsAsync(new Book { Id = 2 });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _bookService.AddNewBookAsync(bookDetails, token));
        Assert.Equal("A book with this ISBN already exists.", ex.Message);
    }
}
