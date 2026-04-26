namespace BookVault.API.DTO.Books
{
    public class BookQueryParameters
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? Search { get; init; }
        public int? CategoryId { get; init; }
        public string? SortBy { get; init; } = "title";
    }
}

