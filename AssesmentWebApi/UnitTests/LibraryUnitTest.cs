using AssesmentWebApi.Controllers;
using AssesmentWebApi.Context;
using AssesmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AssesmentWebApi.Tests
{
    public class LibraryUnitTest
    {
        private readonly DbContextOptions<AssessmentDBContext> _dbContextOptions;

        public LibraryUnitTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AssessmentDBContext>()
                .UseInMemoryDatabase(databaseName: "LibraryUnitTestDB")
                .Options;
        }

        [Fact]
        public async Task AddBook_ValidBook_ReturnsCreatedResult()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new LibraryController(context);
                var bookToAdd = new Book { ISBN = "1234567890", Title = "Meditations", Author = "Marcus Aurelius" };

                // Act
                var result = await controller.AddBook(bookToAdd) as ActionResult<Book>;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(bookToAdd, result.Value);
            }
        }

        [Fact]
        public async Task RemoveBook_ExistingISBN_ReturnsBook()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var bookToRemove = new Book { ISBN = "1234567890", Title = "Meditations", Author = "Marcus Aurelius" };
                context.Books.Add(bookToRemove);
                await context.SaveChangesAsync();
                var controller = new LibraryController(context);

                // Act
                var result = await controller.RemoveBook("1234567890") as ActionResult<Book>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var removedBook = Assert.IsAssignableFrom<Book>(okResult.Value);
                Assert.Equal(bookToRemove, removedBook);
            }
        }

        [Fact]
        public async Task FindBookByTitle_ExistingTitle_ReturnsBook()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var book = new Book { ISBN = "1234567890", Title = "Meditations", Author = "Marcus Aurelius" };
                context.Books.Add(book);
                await context.SaveChangesAsync();
                var controller = new LibraryController(context);

                // Act
                var result = await controller.FindBookByTitle("Meditations") as ActionResult<IEnumerable<Book>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
                Assert.Single(books);
                Assert.Contains(book, books);
            }
        }

        [Fact]
        public async Task FindBookByAuthor_ExistingAuthor_ReturnsBook()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var book = new Book { ISBN = "1234567890", Title = "Meditations", Author = "Marcus Aurelius" };
                context.Books.Add(book);
                await context.SaveChangesAsync();
                var controller = new LibraryController(context);

                // Act
                var result = await controller.FindBookByAuthor("Marcus Aurelius") as ActionResult<IEnumerable<Book>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
                Assert.Single(books);
                Assert.Contains(book, books);
            }
        }

        [Fact]
        public async Task FindBookByISBN_ExistingISBN_ReturnsBook()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var book = new Book { ISBN = "1234567890", Title = "Meditations", Author = "Marcus Aurelius" };
                context.Books.Add(book);
                await context.SaveChangesAsync();
                var controller = new LibraryController(context);

                // Act
                var result = await controller.FindBookByISBN("1234567890") as ActionResult<Book>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var foundBook = Assert.IsAssignableFrom<Book>(okResult.Value);
                Assert.Equal(book, foundBook);
            }
        }

        [Fact]
        public async Task DisplayInfo_ExistingBooks_ReturnsBooks()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var books = new List<Book>
                {
                    new Book {ISBN = "1234567890", Title = "48 Laws Of Power", Author = "Rober Greene" },
                    new Book { ISBN = "0987654321", Title = "Meditations", Author = "Marcus Aurelius" }
                };
                context.Books.AddRange(books);
                await context.SaveChangesAsync();
                var controller = new LibraryController(context);

                // Act
                var result = await controller.DisplayInfo() as ActionResult<IEnumerable<Book>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedBooks = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
                Assert.Equal(books.Count, returnedBooks.Count());
                foreach (var book in books)
                {
                    Assert.Contains(book, returnedBooks);
                }
            }
        }
    }
}
