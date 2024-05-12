using AssesmentWebApi.Context;
using AssesmentWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanManagerController : ControllerBase
    {
        private readonly AssessmentDBContext _dbContext;

        public LoanManagerController(AssessmentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Endpoint for borrowing a book by a user
        /// </summary>
        [HttpPost]
        [Route("CheckoutBook")]
        public async Task<IActionResult> CheckoutBook([FromBody] CheckoutRequest request)
        {
           
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == request.UserID);
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.BookID == request.BookID);

            if (user == null || book == null)
            {
                return BadRequest("Invalid user or book ID.");
            }

            // Check if the book is available for loan
            if (book.AvailableCopies == 0)
            {
                return BadRequest("Book is not available for loan.");
            }

            // Check if the user has already borrowed the book
            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.UserID == user.UserID && loan.BookID == book.BookID);
            if (existingLoan != null)
            {
                return BadRequest("User has already borrowed the book.");
            }

            // Decrease the available copies of the book
            book.AvailableCopies--;

            // Add a new loan
            var newLoan = new Loan
            {
                UserID = user.UserID,
                BookID = book.BookID,

                // Assume that Due date is after 14 days
                DueDate = DateTime.Now.AddDays(14)
            };

            _dbContext.Loans.Add(newLoan);
            await _dbContext.SaveChangesAsync();

            return Ok("Book checked out successfully.");
        }


        /// <summary>
        /// Returning a borrowed book
        /// </summary>
        [HttpPost]
        [Route("ReturnBook")]
        public async Task<ActionResult> ReturnBook(Book book)
        {
            var loan = await _dbContext.Loans.FirstOrDefaultAsync(l => l.BookID == book.BookID);
            if (loan == null)
            {
                return BadRequest("This book is not currently on loan.");
            }

            _dbContext.Loans.Remove(loan);
            await _dbContext.SaveChangesAsync();
            return Ok("Book returned successfully.");
        }

        /// <summary>
        /// Retrieve all books currently loaned by a user
        /// </summary>
        [HttpGet]
        [Route("GetLoansByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetLoansByUser(int userId)
        {
            // Retrieve loans by UserID
            var loans = await _dbContext.Loans.Where(l => l.UserID == userId)
                                              .Select(l => l.Book)
                                              .ToListAsync();

            if (loans == null || loans.Count == 0)
            {
                return NotFound("No books currently loaned by this user.");
            }

            return loans;
        }

        /// <summary>
        /// Retrieve all users currently loaning a specific book
        /// </summary>
        [HttpGet]
        [Route("GetLoansByBook/{bookId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetLoansByBook(int bookId)
        {
            // Retrieve loans by BookID
            var loans = await _dbContext.Loans.Where(l => l.BookID == bookId)
                                              .Select(l => l.User)
                                              .ToListAsync();

            if (loans == null || loans.Count == 0)
            {
                return NotFound("No users currently loaning this book.");
            }

            return loans;
        }
    }
}
