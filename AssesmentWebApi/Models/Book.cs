using Microsoft.AspNetCore.Identity;

namespace AssesmentWebApi.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public int AvailableCopies { get; set; }

    }
}
