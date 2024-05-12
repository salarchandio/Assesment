namespace AssesmentWebApi.Models
{
    public class Loan
    {
        public int LoanID { get; set; }
        public int UserID { get; set; }
        public int BookID { get; set; }
        public User User { get; set; }
        public Book Book { get; set; }
        public DateTime DueDate { get; set; }
    }
}
