using FluentValidation;
using ShopApp.Application.Dtos;

namespace ShopApp.Application.Validators;

public class CategoryFormDtoValidator : AbstractValidator<CategoryFormDto>
{
    public CategoryFormDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}