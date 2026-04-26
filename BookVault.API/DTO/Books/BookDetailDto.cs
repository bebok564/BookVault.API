using BookVault.API.DTO.Authors;
using BookVault.API.DTO.Reviews;
using BookVault.API.Models;

namespace BookVault.API.DTO.Books
{
    public record BookDetailDto(
        int Id,
        string Title,
        string Isbn,
        string? Description,
        int PageCount,
        DateOnly PublishedDate,
        decimal AverageRating,        // brakuje
        AuthorDto Author,             // pełny obiekt, nie string
        List<string> Categories,
        List<ReviewDto> Reviews       // brakuje
    );
}
