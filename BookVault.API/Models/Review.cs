namespace BookVault.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } //1-5
        public string? Comment { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
