using BookVault.API.DTO.Books;
using BookVault.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace BookVault.API.Validators
{
    public class CreateBookValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookValidator()
        {
            //Title — nie pusty, max 200 znaków
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200);

           // PageCount — większy od 0, mniejszy od 10000
            RuleFor(x => x.PageCount)
                .GreaterThan(0)
                .LessThan(10000);

             // Isbn — nie pusty, 10 lub 13 cyfr
            RuleFor(x => x.Isbn)
                .NotEmpty()
                .Matches(@"^\d{10}(\d{3})?$")
                .WithMessage("ISBN must be 10 or 13 digits");
           
          //AuthorId — większy od 0
          RuleFor(x => x.AuthorId)
                .GreaterThan(0);
            //CategoryIds — nie pusta lista
          RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("At least one category is required");
        }
    }
}
