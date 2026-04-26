namespace BookVault.API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public int PageCount { get; set; }
        public DateOnly PublishedDate { get; set; }
        public decimal AverageRating { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public ICollection<Category> Categories { get; set; }  = new List<Category>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
