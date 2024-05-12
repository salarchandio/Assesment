using AssesmentWebApi.Context;
using AssesmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly AssessmentDBContext _dbContext;

        public LibraryController(AssessmentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a book to the library
        /// </summary>

        [HttpPost]
        [Route("AddBook")]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(FindBookByISBN), new { isbn = book.ISBN }, book);
        }

        /// <summary>
        /// Remove a book from the library by isbn
        /// </summary>

        [HttpDelete]
        [Route("RemoveBook/{isbn}")]
        public async Task<ActionResult<Book>> RemoveBook(string isbn)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            if (book == null)
            {
                return NotFound();
            }

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        /// <summary>
        /// Find books by title
        /// </summary>

        [HttpGet]
        [Route("FindBookByTitle/{title}")]
        public async Task<ActionResult<IEnumerable<Book>>> FindBookByTitle(string title)
        {
            var books = await _dbContext.Books.Where(b => b.Title.Contains(title)).ToListAsync();
            if (books == null || books.Count == 0)
            {
                return NotFound();
            }

            return books;
        }

        /// <summary>
        /// Find books by author
        /// </summary>

        [HttpGet]
        [Route("FindBookByAuthor/{author}")]
        public async Task<ActionResult<IEnumerable<Book>>> FindBookByAuthor(string author)
        {
            var books = await _dbContext.Books.Where(b => b.Author == author).ToListAsync();
            if (books == null || books.Count == 0)
            {
                return NotFound();
            }

            return books;
        }

        /// <summary>
        /// Find a book by ISBN
        /// </summary>

        [HttpGet]
        [Route("FindBookByISBN/{isbn}")]
        public async Task<ActionResult<Book>> FindBookByISBN(string isbn)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        /// <summary>
        /// Display all books in the library
        /// </summary>
        [HttpGet]
        [Route("DisplayInfo")]
        public async Task<ActionResult<IEnumerable<Book>>> DisplayInfo()
        {
            var books = await _dbContext.Books.ToListAsync();

            if (books == null || books.Count == 0)
            {
                return NotFound();
            }

            return books;
        }
    }
}
