namespace BookVault.API.DTO.Authors
{ 
    public record CreateAuthorDto(
    string FirstName,
    string LastName,
    string? Bio,
    DateOnly? BirthDate
    );
}
