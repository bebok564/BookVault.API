namespace BookVault.API.DTO.Authors
{
    public record UpdateAuthorDto(
    string FirstName,
    string LastName,
    string? Bio,
    DateOnly? BirthDate
   );
    
}
