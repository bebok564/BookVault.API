using BookVault.API.Models;

namespace BookVault.API.DTO.Books
{
    public record BookDto(
      int Id,
      string Title,
      string Isbn,
      int PageCount,
      string AuthorName,     // Nie cały obiekt Author!
      decimal AverageRating,
      List<string> Categories  // Tylko nazwy, nie encje
  );
}
