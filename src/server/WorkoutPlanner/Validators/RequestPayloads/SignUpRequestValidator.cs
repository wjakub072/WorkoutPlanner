using FluentValidation;
using WorkoutPlanner.Api.Data.Requests;

namespace WorkoutPlanner.Api.Validators.RequestPayloads;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(2, 12).WithMessage("Username must be between 2 and 12 characters.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
