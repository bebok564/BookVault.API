using BookVault.API.DTO.Reviews;
using BookVault.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace BookVault.API.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            //Rating — od 1 do 5
            RuleFor(x => x.Rating)
                .InclusiveBetween(1,5).WithMessage("Rating must be between 1 and 5");
            //ReviewerName — nie pusty, max 100 znaków
            RuleFor(x => x.ReviewerName)
                .NotEmpty().WithMessage("Reviewer name is required");
        }
    }
}
