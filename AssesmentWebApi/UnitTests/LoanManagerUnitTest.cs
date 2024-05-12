using AssesmentWebApi.Context;
using AssesmentWebApi.Controllers;
using AssesmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AssesmentWebApi.UnitTests
{
    public class LoanManagerUnitTest
    {
        private readonly DbContextOptions<AssessmentDBContext> _dbContextOptions;

        public LoanManagerUnitTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AssessmentDBContext>()
                .UseInMemoryDatabase(databaseName: "Test_LoanManagerDB")
                .Options;
        }

        [Fact]
        public async Task CheckoutBook_ValidRequest_ReturnsOk()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new LoanManagerController(context);
                var user = new User { UserID = 1, Name = "Salar ALi" };
                var book = new Book { BookID = 1, Title = "Meditations", AvailableCopies = 1 };
                await context.Users.AddAsync(user);
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();

                var request = new CheckoutRequest { UserID = user.UserID, BookID = book.BookID };

                // Act
                var result = await controller.CheckoutBook(request) as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Book checked out successfully.", result.Value);
            }
        }

        [Fact]
        public async Task ReturnBook_BookOnLoan_ReturnsOk()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new LoanManagerController(context);
                var user = new User { UserID = 1, Name = "Salar Ali" };
                var book = new Book { BookID = 1, Title = "Meditations", AvailableCopies = 1 };
                var loan = new Loan { LoanID = 1, UserID = user.UserID, BookID = book.BookID, DueDate = DateTime.Now.AddDays(14) };
                await context.Users.AddAsync(user);
                await context.Books.AddAsync(book);
                await context.Loans.AddAsync(loan);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.ReturnBook(book) as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Book returned successfully.", result.Value);
            }
        }

        [Fact]
        public async Task GetLoansByUser_UserWithLoans_ReturnsListOfBooks()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new LoanManagerController(context);
                var user = new User { UserID = 1, Name = "Salar Ali" };
                var book1 = new Book { BookID = 1, Title = "Meditations", AvailableCopies = 1 };
                var book2 = new Book { BookID = 2, Title = "48 Laws Of Power", AvailableCopies = 1 };
                var loan1 = new Loan { LoanID = 1, UserID = user.UserID, BookID = book1.BookID, DueDate = DateTime.Now.AddDays(14) };
                var loan2 = new Loan { LoanID = 2, UserID = user.UserID, BookID = book2.BookID, DueDate = DateTime.Now.AddDays(14) };
                await context.Users.AddAsync(user);
                await context.Books.AddRangeAsync(new[] { book1, book2 });
                await context.Loans.AddRangeAsync(new[] { loan1, loan2 });
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetLoansByUser(user.UserID) as ActionResult<IEnumerable<Book>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
                Assert.Equal(2, books.Count());
            }
        }

        [Fact]
        public async Task GetLoansByBook_BookWithLoans_ReturnsListOfUsers()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new LoanManagerController(context);
                var user1 = new User { UserID = 1, Name = "Salar Ali" };
                var user2 = new User { UserID = 2, Name = "Mansoor ALi" };
                var book = new Book { BookID = 1, Title = "Meditations", AvailableCopies = 1 };
                var loan1 = new Loan { LoanID = 1, UserID = user1.UserID, BookID = book.BookID, DueDate = DateTime.Now.AddDays(14) };
                var loan2 = new Loan { LoanID = 2, UserID = user2.UserID, BookID = book.BookID, DueDate = DateTime.Now.AddDays(14) };
                await context.Users.AddRangeAsync(new[] { user1, user2 });
                await context.Books.AddAsync(book);
                await context.Loans.AddRangeAsync(new[] { loan1, loan2 });
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetLoansByBook(book.BookID) as ActionResult<IEnumerable<User>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var users = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
                Assert.Equal(2, users.Count());
            }
        }
    }
}
