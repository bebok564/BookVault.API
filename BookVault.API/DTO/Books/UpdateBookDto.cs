namespace BookVault.API.DTO.Books
{
    public record UpdateBookDto(
      string Title,
      string Isbn,
      string? Description,
      int PageCount,
      DateOnly PublishedDate,
      int AuthorId,
      List<int> CategoryIds
    );
    
}
