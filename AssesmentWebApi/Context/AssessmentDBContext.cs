using AssesmentWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AssesmentWebApi.Context
{
    public class AssessmentDBContext : DbContext
    {
        public AssessmentDBContext(DbContextOptions<AssessmentDBContext> options) : base(options)
        {
            //This optimization improves performance by avoiding the overhead of tracking changes
            //suitable for read-only scenarios or when entities are not modified.
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            //This prevents unexpected additional database queries and helps avoid performance issues
            //especially in scenarios involving large datasets or frequent database interactions
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }
}
