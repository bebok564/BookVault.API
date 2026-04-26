using BookVault.API.Models;

namespace BookVault.API.DTO.Reviews
{
    public record ReviewDto(
        int Id,
        string ReviewerName,
        string? Comment,
        int Rating,
        DateTime CreatedAt
    );

}
