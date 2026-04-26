namespace BookVault.API.DTO.Reviews
{
    public record CreateReviewDto(
        
        int Rating,
        string? Comment,
        string ReviewerName

    );

}
