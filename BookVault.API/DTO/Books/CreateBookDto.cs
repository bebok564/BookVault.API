namespace BookVault.API.DTO.Books
{
    public record CreateBookDto(
     string Title,
     string Isbn,
     string? Description,
     int PageCount,
     DateOnly PublishedDate,
     int AuthorId,           // Tylko ID, nie cały autor
     List<int> CategoryIds   // Lista ID kategorii
    );
}
