namespace BookVault.API.DTO.Authors
{
    public record AuthorDto(
    int Id,
    string FirstName,
    string LastName,
    string? Bio
);
}
