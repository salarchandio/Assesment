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
    public class UserManagerController : ControllerBase
    {
        private readonly AssessmentDBContext _dbContext;

        public UserManagerController(AssessmentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a user
        /// </summary>
        [HttpPost]
        [Route("AddUser")]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(FindUserByEmail), new { email = user.Email }, user);
        }

        /// <summary>
        /// Remove a user
        /// </summary>
        [HttpDelete]
        [Route("RemoveUser/{email}")]
        public async Task<ActionResult> RemoveUser(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Find a user by email
        /// </summary>
        [HttpGet]
        [Route("FindUserByEmail/{email}")]
        public async Task<ActionResult<User>> FindUserByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Display all users
        /// </summary>
        [HttpGet]
        [Route("DisplayInfo")]
        public async Task<ActionResult<IEnumerable<User>>> DisplayInfo()
        {
            var users = await _dbContext.Users.ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            return users;
        }
    }
}
