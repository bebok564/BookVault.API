using BookVault.API.DTO.Authors;
using FluentValidation;

namespace BookVault.API.Validators
{
    public class CreateAuthorValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorValidator()
        {
            //FirstName — nie pusty, max 100 znaków
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");
            //LastName — nie pusty, max 100 znaków
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");
        }
    }
}
