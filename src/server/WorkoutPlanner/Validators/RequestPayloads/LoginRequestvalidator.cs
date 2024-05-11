using FluentValidation;
using WorkoutPlanner.Api.Data.Requests;

namespace WorkoutPlanner.Api.Validators.RequestPayloads;

public class LoginRequestvalidator : AbstractValidator<LoginRequest>
{
    public LoginRequestvalidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email is required.");
        RuleFor(request => request.Password).NotEmpty().WithMessage("Password is required.");
    }
}
