using AssesmentWebApi.Context;
using AssesmentWebApi.Controllers;
using AssesmentWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace AssesmentWebApi.UnitTests
{
    public class UserManagerUnitTest
    {
        private readonly DbContextOptions<AssessmentDBContext> _dbContextOptions;

        public UserManagerUnitTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AssessmentDBContext>()
                .UseInMemoryDatabase(databaseName: "UserManagerUnitTestDB")
                .Options;
        }

        [Fact]
        public async Task AddUser_ValidUser_ReturnsCreatedResult()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var controller = new UserManagerController(context);
                var userToAdd = new User { Email = "salar.chandio@gmail.com", Name = "Salar Ali" };

                // Act
                var result = await controller.AddUser(userToAdd) as ActionResult<User>;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userToAdd, result.Value);
            }
        }

        [Fact]
        public async Task RemoveUser_ExistingEmail_ReturnsNoContent()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var userToRemove = new User { Email = "salar.chandio@gmail.com", Name = "Salar Ali"};
                context.Users.Add(userToRemove);
                await context.SaveChangesAsync();
                var controller = new UserManagerController(context);

                // Act
                var result = await controller.RemoveUser("salar.chandio@gmail.com") as ActionResult;

                // Assert
                Assert.NotNull(result);
                var noContentResult = Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task FindUserByEmail_ExistingEmail_ReturnsUser()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var user = new User { Email = "salar.chandio@gmail.com", Name = "Salar Ali"};
                context.Users.Add(user);
                await context.SaveChangesAsync();
                var controller = new UserManagerController(context);

                // Act
                var result = await controller.FindUserByEmail("salar.chandio@gmail.com") as ActionResult<User>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var foundUser = Assert.IsAssignableFrom<User>(okResult.Value);
                Assert.Equal(user, foundUser);
            }
        }

        [Fact]
        public async Task DisplayInfo_ExistingUsers_ReturnsUsers()
        {
            // Arrange
            using (var context = new AssessmentDBContext(_dbContextOptions))
            {
                var users = new List<User>
                {
                    new User { Email = "salar.chandio@gmail.com", Name = "Salar Ali"},
                    new User { Email = "mansoor.chandio@gmail.com", Name = "Mansoor Ali"}
                };
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
                var controller = new UserManagerController(context);

                // Act
                var result = await controller.DisplayInfo() as ActionResult<IEnumerable<User>>;

                // Assert
                Assert.NotNull(result);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
                Assert.Equal(users.Count, returnedUsers.Count());
                foreach (var user in users)
                {
                    Assert.Contains(user, returnedUsers);
                }
            }
        }
    }
}
